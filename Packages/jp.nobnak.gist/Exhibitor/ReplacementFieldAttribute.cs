namespace nobnak.Gist.Exhibitor {

    public class ReplacementFieldAttribute : System.Attribute {

        public readonly string nameField;

        public ReplacementFieldAttribute(string nameField) {
            this.nameField = nameField;
        }
    }
}
