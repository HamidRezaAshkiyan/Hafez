using HafezLibrary.Models;
using HafezLibrary.Security;
using HafezLibrary.Validators;
using Xunit;

namespace HafezLibrary.Tests
{
    public class SecurityAndValidationTests
    {
        [Fact]
        public void PasswordHasher_HashAndVerify_ShouldSucceedAndUseUniqueSalts()
        {
            string hash1 = PasswordHasher.Hash("MySecret123!");
            string hash2 = PasswordHasher.Hash("MySecret123!");

            Assert.NotEqual(hash1, hash2);
            Assert.True(PasswordHasher.IsHashed(hash1));
            Assert.True(PasswordHasher.Verify("MySecret123!", hash1));
            Assert.True(PasswordHasher.Verify("MySecret123!", hash2));
            Assert.False(PasswordHasher.Verify("WrongPassword", hash1));
        }

        [Fact]
        public void PasswordHasher_LegacyPlaintextFallback_ShouldVerify()
        {
            Assert.False(PasswordHasher.IsHashed("legacy123"));
            Assert.True(PasswordHasher.Verify("legacy123", "legacy123"));
            Assert.False(PasswordHasher.Verify("other", "legacy123"));
        }

        [Fact]
        public void AddUserValidator_ShouldRejectEmptyAndOversizedUserId()
        {
            var validator = new AddUserValidator();

            Assert.False(validator.Validate(new UserModel { UserId = "" }).IsValid);
            Assert.False(validator.Validate(new UserModel { UserId = "12345678901" }).IsValid);
            Assert.True(validator.Validate(new UserModel { UserId = "operator1" }).IsValid);
        }

        [Fact]
        public void NotificationValidator_ShouldRequireDescription()
        {
            var validator = new NotificationValidator();

            Assert.False(validator.Validate(new NotificationModel { Description = "" }).IsValid);
            Assert.True(validator.Validate(new NotificationModel { Description = "اعلان تست" }).IsValid);
        }

        [Fact]
        public void NotificationGroupValidator_ShouldValidateNameLength()
        {
            var validator = new NotificationGroupValidator();

            Assert.False(validator.Validate(new NotificationGroupModel { Name = "" }).IsValid);
            Assert.False(validator.Validate(new NotificationGroupModel { Name = "A" }).IsValid);
            Assert.True(validator.Validate(new NotificationGroupModel { Name = "اعلانات مسجد" }).IsValid);
        }
    }
}
