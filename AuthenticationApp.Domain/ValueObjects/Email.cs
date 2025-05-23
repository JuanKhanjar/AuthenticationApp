namespace AuthenticationApp.Domain.ValueObjects
{
    /// <summary>
    /// قيمة كائنية تمثل البريد الإلكتروني
    /// </summary>
    public readonly record struct Email(string Value)
    {
        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new ArgumentException("Invalid email format.", nameof(value));

            return new Email(value);
        }
    }
}
