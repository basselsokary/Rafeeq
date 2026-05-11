namespace Application.DTOs.Artifacts;

public record ScanImageResponse(
    List<ScanImageResult> Results);

public record ScanImageResult(
    string VectorId,
    string ArtifactId,
    double Distance);
