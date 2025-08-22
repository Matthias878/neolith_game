public class Culture
{
    private string name;
    private enum Housing { Communal, Individual }
    private Housing housingPreference; //Start with communal
    private enum Architectural_Style { Round, Rectangular }
    private Architectural_Style architecturalPreference; //?

    public Technology[] technologies = new Technology[0];

    public Culture(string name)
    {
        // Initialize culture with a name and default preferences
        this.name = name;
        this.housingPreference = Housing.Communal; // Default to communal housing
        this.architecturalPreference = Architectural_Style.Round; // Default architectural style
    }

    public override string ToString()
    {
        // Return a string representation of the culture
        return $"Culture: {name}, Housing: {housingPreference}, Architecture: {architecturalPreference} The Culture of the {name} has a long way to go.";
    }
}


//Technologie integrated Start with Fire

//Ai starts with more (or less technology)

//technologies are not binary they can be known better with time

//Start with Ackerbau? je nach pflanze? you can one planttype at start of game
//your culture preferes sesshaftigkeit or normadentum
//Keramik
//Textilien weben von Wolle/Flachs
//Steinbearbeitung glätten und schleifen von Steingeräten
//das Rad?
//Bergbau & Metallurgie Kupfer -> Bronze

// Mahlsteine
//Backöfen
//Flöß
//Kalender
//bessere Waffen? schwert beil etc..
//Metallhandwerk, Gießformen etc.
//Schrift

//Soziale technologien? (Vorratswirtschaft, Tauschhandel, Arbeitsteilung)
//befestigte Siedlungen?

//Your culture speaks a language
//Your language has a predominent faith (Eventchain if split into different one, growing a different one, multiple coming)

//each culture has an opinion of other cultures <-> other cultures clans and how close they live together, relationship influences own cultural preference over time

//what needs to happen to learn new technologies? 1.Live with or see somebody using it 2. Experiment with materials and techniques? 3. very good in one technology leads to another
//proficiency in one technology is increased by use
//some technologies are just subtechnologies of others (keramik von feuer?)

//after first settlement establish architectural preference

//Verzierungs präferenzen entstehen durch availibility and seeing what other (more advanced?) cultures do.

//Your culture is in different Ages? final age bronze age

//Cultures have traits that make them unique, such as architectural styles, social organization, and technological advancements.

//you start with a settlment, that has access to at least one plant, your culture knows how to grow

//others of your culture have not settled down

//your culture has leader inheritance and normal inheritance rules (communal housing vs individual housing has an influence)

//when you lead your culture (most clans of your culture follow you) you can for varying costs try to change something about your culture
