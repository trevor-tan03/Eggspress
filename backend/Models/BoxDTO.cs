using backend.util;

public record BoxDTO(string code, DateTime expiresAt, long boxSize, List<FileDTO> files);