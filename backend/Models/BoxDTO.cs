using backend.util;

public record BoxDTO(string code, DateTime expiresAt, List<FileDTO> files);