using Models;

namespace Suggestions
{
    public static class BMISuggestionGenerator
    {
        public static List<Suggestion> CreateSuggestions(User user)
        {
            var userMass = user.Weight;
            var userHeight = user.Height;

            var bmi = userMass / (userHeight * userHeight);

            var suggestions = new List<Suggestion>();

            switch (bmi)
            {
                case < 18.5d:
                    suggestions.Add(new Suggestion("UnderWeight", "You should eat more bro..... look at yourself... weak plebian. :(", DateTime.Now, AspectType.General));

                    suggestions.Add(new Suggestion("Balanced high-calorie diet", "Increase calorie intake through a balanced diet rich in healthy fats, proteins, and carbohydrates. Try eating more meals throughout the day.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Appropriate physical activity", "Focus on exercises that help build muscle mass, such as strength training.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Consultation with a dietitian", "Consider consulting a dietitian who can help develop a nutrition plan tailored to your individual needs.", DateTime.Now, AspectType.General));
                    break;
                case >= 18.5d and < 24.9:
                    suggestions.Add(new Suggestion("NormalWeight", "Good job dude.... just good job. <3", DateTime.Now, AspectType.General));

                    suggestions.Add(new Suggestion("Maintaining a healthy diet", "Continue consuming balanced meals rich in vegetables, fruits, proteins, and healthy fats.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Regular physical activity", "Maintain regular physical activity to stay fit and healthy. It is recommended to have at least 150 minutes of moderate aerobic activity per week.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Health monitoring", "Regular check-ups can help detect potential health issues early.", DateTime.Now, AspectType.General));
                    break;
                case >= 25d:
                    suggestions.Add(new Suggestion("ToFatWeight", "You are to fat. Sorry. Nothing we can do about it.", DateTime.Now, AspectType.General));

                    suggestions.Add(new Suggestion("Healthy and controlled diet", "Reduce calorie intake by making healthier food choices. Focus on eating vegetables, fruits, lean proteins, and limit saturated fats and sugars.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Regular physical activity", "Increase physical activity by engaging in aerobic exercises and strength training to burn calories and build muscles.", DateTime.Now, AspectType.General));
                    suggestions.Add(new Suggestion("Professional support", "Consider consulting a doctor, dietitian, or personal trainer to get professional support and a personalized action plan.", DateTime.Now, AspectType.General));
                    break;
            }

            return suggestions;
        }
    }
}
