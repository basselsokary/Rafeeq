using Shared;

namespace Domain.Common;

public static class UserErrors
{
    public static Error Unauthorized(string? userId) =>
        Error.Unauthorized("USER_UNAUTHORIZED", $"The user with ID '{userId}' is unauthorized.");

    public static Error NotFound(string key)
        => Error.NotFound("USER_NOT_FOUND", $"User with '{key}' was not found.");

    public static Error IdRequired =>
        Error.Validation("USER_ID_REQUIRED", "User ID is required.");
    public static Error FullNameRequired =>
        Error.Validation("USER_FULL_NAME_REQUIRED", "Full name is required.");
    
    public static Error FirstNameRequired =>
        Error.Validation("USER_FIRST_NAME_REQUIRED", "First name is required.");

    public static Error LastNameRequired =>
        Error.Validation("USER_LAST_NAME_REQUIRED", "Last name is required.");
    
    public static Error FirstNameExceededLength =>
        Error.Validation("USER_FIRST_NAME_LENGTH_EXCEEDED", "User first name exceeds the maximum allowed length.");

    public static Error LastNameExceededLength =>
        Error.Validation("USER_LAST_NAME_LENGTH_EXCEEDED", "User last name exceeds the maximum allowed length.");

    public static Error RoleInvalid =>
        Error.Validation("USER_ROLE_INVALID", "Selected role is invalid.");
    
    public static Error UserNameRequired =>
        Error.Validation("USER_USERNAME_REQUIRED", "Username is required.");

    public static Error UserNameExceededLength =>
        Error.Validation("USER_USERNAME_LENGTH_EXCEEDED", "Username exceeds the maximum allowed length.");

    public static Error PasswordRequired =>
        Error.Validation("USER_PASSWORD_REQUIRED", "Password is required.");

    public static Error PasswordTooShort =>
        Error.Validation("USER_PASSWORD_TOO_SHORT", "Password does not meet the minimum required length.");

    public static Error PasswordTooLong =>
        Error.Validation("USER_PASSWORD_TOO_LONG", "Password exceeds the maximum allowed length.");

    public static Error GoogleIdTokenRequired =>
        Error.Validation("USER_GOOGLE_ID_TOKEN_REQUIRED", "Google ID token is required.");
    
    public static Error InvalidGoogleIdToken =>
        Error.Validation("USER_INVALID_GOOGLE_ID_TOKEN", "The provided Google ID token is invalid.");
}

public static class ValidationErrors
{
    public static Error ValueRequired =>
        Error.Validation("VALIDATION_VALUE_REQUIRED", "Value is required.");

    public static Error CollectionRequired =>
        Error.Validation("VALIDATION_COLLECTION_REQUIRED", "At least one item is required.");

    public static Error InvalidEnumValue =>
        Error.Validation("VALIDATION_INVALID_ENUM_VALUE", "Invalid value was provided.");

    public static Error NumberMustBeGreaterThanZero =>
        Error.Validation("VALIDATION_NUMBER_GREATER_THAN_ZERO_REQUIRED", "Value must be greater than zero.");

    public static Error MaximumLengthExceeded =>
        Error.Validation("VALIDATION_MAXIMUM_LENGTH_EXCEEDED", "Value exceeds the maximum allowed length.");

    public static Error MinimumLengthNotMet =>
        Error.Validation("VALIDATION_MINIMUM_LENGTH_NOT_MET", "Value does not meet the minimum required length.");

    public static Error RangeInvalid() =>
        Error.Validation("VALIDATION_RANGE_INVALID", "The provided range is invalid.");
    
    public static Error RangeInvalid(string min, string max) =>
        Error.Validation("VALIDATION_RANGE_INVALID", $"The provided range is invalid. Please provide a value between {min} and {max}.");

    public static Error WhitespaceNotAllowed =>
        Error.Validation("VALIDATION_WHITESPACE_NOT_ALLOWED", "Whitespace-only values are not allowed.");
}

public static class ImageErrors
{
    public static Error ImageIdRequired =>
        Error.Validation("IMAGE_ID_REQUIRED", "Image ID is required.");

    public static Error InvalidSignature =>
        Error.Validation("IMAGE_INVALID_SIGNATURE", "The uploaded file has an invalid signature and may not be a valid image.");

    public static Error UploadFailed =>
        Error.Failure("IMAGE_UPLOAD_FAILED", "An error occurred while uploading the image. Please try again later.");
    
    public static Error StorageKeyRequired =>
        Error.Validation("IMAGE_STORAGE_KEY_REQUIRED", "Storage key is required.");

    public static Error NegativeDisplayOrder =>
        Error.Validation("IMAGE_NEGATIVE_DISPLAY_ORDER", "Display order cannot be negative.");
    
    public static Error ImageUrlRequired =>
        Error.Validation("IMAGE_URL_REQUIRED", "Image URL is required.");

    public static Error ProcessingFailed =>
        Error.Failure("IMAGE_PROCESSING_FAILED", "Image could not be processed.");
}

public static class FileErrors
{
    public static readonly Error InvalidId =
        Error.Validation("FILE_INVALID_ID", "File ID must not be empty.");

    public static readonly Error InvalidName =
        Error.Validation("FILE_INVALID_NAME", "File name is invalid or empty.");

    public static readonly Error EmptyFile =
        Error.Validation("FILE_EMPTY", "File contains no data.");

    public static readonly Error InvalidSignature =
        Error.Validation("FILE_INVALID_SIGNATURE", "File content does not match its declared type.");

    public static Error FileTooLarge(long maxBytes) =>
        Error.Validation("FILE_TOO_LARGE", $"File exceeds the maximum allowed size of {maxBytes / 1024 / 1024} MB.");

    public static Error UnsupportedType(string ext) =>
        Error.Validation("FILE_UNSUPPORTED_TYPE", $"File type '{ext}' is not supported.");
}