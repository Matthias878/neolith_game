public class Gift : Interaction
{
    public int value;

    public Gift(Person sender, Person intendedRecipient, int value) : base(sender, intendedRecipient)
    {
        this.value = value;
    }
}
