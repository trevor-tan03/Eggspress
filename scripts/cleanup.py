import os
import shutil
import sqlite3
import time
from datetime import datetime

import schedule

CURR_DIR = os.path.dirname(os.path.abspath(__file__))
DB_PATH = os.path.join(CURR_DIR, "../backend/boxes.db")
DB_PATH = os.path.normpath(DB_PATH)

BOXES_PATH = os.path.join(CURR_DIR, "../backend/Boxes")
BOXES_PATH = os.path.normpath(BOXES_PATH)


def remove_expired_boxes():
    try:
        con = sqlite3.connect(DB_PATH)
        cur = con.cursor()
        print("Checking for expired boxes...")

        now = datetime.now().isoformat()

        cur.execute("SELECT Code FROM Boxes WHERE ExpiresAt < ?", (now,))
        expired_boxes = [row[0] for row in cur.fetchall()]

        for code in expired_boxes:
            box_path = os.path.normpath(os.path.join(BOXES_PATH, code))
            if os.path.exists(box_path):
                shutil.rmtree(box_path)
                print(f"Deleted folder: {box_path}")

            cur.execute("DELETE FROM Boxes WHERE Code = ?", (code,))
            print(f"Deleted DB entry for box: {code}")

        con.commit()
    except Exception as e:
        print(f"Error during cleanup: {e}")
    finally:
        cur.close()
        con.close()


schedule.every().hour.do(remove_expired_boxes)

while True:
    schedule.run_pending()
    time.sleep(1)
