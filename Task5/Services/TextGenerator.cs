using Bogus;
using MathNet.Numerics.Random;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
namespace Task5.Services
{
    public class TextGenerator
    {
        private Faker faker;
        private Bogus.DataSets.Lorem lorem;

        private readonly int LessYear = 1950;

        private readonly int MaxYear = 1950;

        private readonly int minISBN = 1;

        private readonly int maxISBN = 99999;

        private readonly int mod = 8;

        private bool value = false;

        public TextGenerator()
        {
            faker = new Faker("vi");
            lorem = new Bogus.DataSets.Lorem(locale: "vi");

        }
        public string GetAuthor()
        {
            return faker.Random.Bool() ? faker.Name.FullName() : faker.Name.FullName() + ", " + faker.Name.FullName();
        }
        public void ChangeLocale(string locale)
        {
            faker = new Faker(locale);
            lorem = new Bogus.DataSets.Lorem(locale);
        }
        public void ChangeSeed(int seed, int bias)
        {
            Randomizer.Seed = new WH2006(seed + bias);
            faker = new Faker(faker.Locale);
            lorem = new Bogus.DataSets.Lorem(faker.Locale);
        }

        public int GenerateSeed()
        {
            return faker.Random.Int(0, 1000000);
        }

        public string Title()
        {
            return faker.Locale == ("en") ? faker.Company.CatchPhrase() + "." : lorem.Sentence(1, 3).TrimEnd('.');
        }

        public string PublisherNameWithDate()
        {
            return faker.Company.CompanyName() + ", " + faker.Random.Int(LessYear, MaxYear).ToString();
        }

        public string ISBN()
        {
            int FirstValue = faker.Random.Int(minISBN, maxISBN);
            int Powmod = (int)Math.Log10(FirstValue);
            int SecondValue = faker.Random.Int(1, CreateMaxValue(mod - Powmod));
            return "978-" + faker.Random.Int(1, 9).ToString() + "-" + FirstValue.ToString() + "-" + SecondValue.ToString() + "-" + faker.Random.Int(1, 9).ToString();
        }
        public int CreateMaxValue(int mod)
        {
            int res = 0;
            while (mod > 0)
            {
                res *= 10;
                res += 9;
                mod--;
            }
            return res;
        }

        public int GetLikes(double likes)
        {
            return faker.Random.Double(0, 1) < likes % 1 ? 1 + (int)Math.Floor(likes) : 0 + (int)Math.Floor(likes);
        }

        public List<string> GetSetOfReviewsAuthor(double reviews)
        {
            List<string> rews = new List<string>();
            for (int i = 0; i < Math.Floor(reviews); ++i) rews.Add(faker.Name.FullName());
            if (faker.Random.Double(0, 1) < reviews - Math.Floor(reviews))
            {
                rews.Add(faker.Name.FullName());
                value = true;
            }
            return rews;
        }
        public List<string> GetSetOfReviewsText(double reviews)
        {
            List<string> rews = new List<string>();
            if (faker.Locale != "en") for (int i = 0; i < Math.Floor(reviews); ++i) rews.Add(faker.Lorem.Sentence());
            else rews = faker.Rant.Reviews("book", (int)Math.Floor(reviews)).ToList<string>();
            if (value) rews.Add(faker.Locale == ("en") ? faker.Hacker.Phrase() : faker.Lorem.Sentence());
            value = false;
            return rews;
        }

    }
}
