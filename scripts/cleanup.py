import datetime
import os
import shutil
import sqlite3
import tempfile
import time

import schedule

CURR_DIR = os.path.dirname(os.path.abspath(__file__))
DB_PATH = os.path.normpath(os.path.join(CURR_DIR, "../backend/boxes.db"))
BOXES_PATH = os.path.normpath(os.path.join(tempfile.gettempdir(), "Boxes"))
UPLOADS_PATH = os.path.normpath(os.path.join(tempfile.gettempdir(), "uploads"))


def remove_expired_boxes():
    try:
        con = sqlite3.connect(DB_PATH)
        con.row_factory = sqlite3.Row
        cur = con.cursor()
        cur.execute("PRAGMA foreign_keys = ON;")

        now = datetime.datetime.now(datetime.UTC)
        now_str = now.strftime("%Y-%m-%d %H:%M:%S.%f")
        print("==========")
        print("Now:", now_str)
        print("Checking for expired boxes...")

        cur.execute("SELECT Code FROM Boxes WHERE ExpiresAt < ?", (now_str,))
        expired_boxes = cur.fetchall()

        for row in expired_boxes:
            code = row["Code"]

            # Delete stale uploads for this box
            cur.execute(
                """
                SELECT Id FROM Files 
                WHERE BoxCode = ?
            """,
                (code,),
            )
            for file_row in cur.fetchall():
                file_id = file_row["Id"]
                upload_path = os.path.join(UPLOADS_PATH, file_id)
                if os.path.exists(upload_path):
                    shutil.rmtree(upload_path)
                    print(f"Deleted temp upload chunks: {upload_path}")

                cur.execute(
                    "DELETE FROM Files WHERE Id = ? AND BoxCode = ?",
                    (file_id, code),
                )
                print(f"Deleted DB entry for incomplete file: {file_id}")

            # Delete box folder and DB record
            box_path = os.path.join(BOXES_PATH, code)
            if os.path.exists(box_path):
                shutil.rmtree(box_path)
                print(f"Deleted box folder: {box_path}")

            cur.execute("DELETE FROM Boxes WHERE Code = ?", (code,))
            print(f"Deleted box DB entry: {code}")

        con.commit()

    except Exception as e:
        print(f"Error during cleanup: {e}")
    finally:
        cur.close()
        con.close()


schedule.every(10).minutes.do(remove_expired_boxes)

while True:
    schedule.run_pending()
    time.sleep(1)
