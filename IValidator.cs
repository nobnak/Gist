namespace nobnak.Gist {
	public interface IValidator {
		bool IsValid { get; }

		void Invalidate();
		bool Validate(bool force = false);
	}
}