using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace ModularEncountersSpawner {
	
	public static class RandomNameGenerator{
		
		public static Random Rnd = new Random();
		
		public static string CharStringAll = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		public static string CharStringNumbers = "0123456789";
		public static string CharStringLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		
		public static string[] GoodAdjectives = {

			"Gilded", 
			"Privilaged", 
			"Honorable", 
			"Heroic", 
			"Brave", 
			"Giddy", 
			"Happy",	
			"Kind",	
			"Determined", 
			"Excited", 
			"Satisfied", 
			"Patriotic",
			"Wishful"
		};
		public static string[] NeutralAdjectives = {

			"Cautious",
			"Timid",
			"Stubborn",
			"Bored",
			"Stoic",
			"Shivering",
			"Sweating",
			"Lonely",
			"Lone"
		};
		public static string[] BadAdjectives = {

			"Dishonorable",
			"Filthy",
			"Broken",
			"Shunned",
			"Corrupt",
			"Contaminated",
			"Compromised",
			"Cowardly",
			"Gloomy",
			"Sour",
			"Problematic",
			"Morose",
			"Glum",
			"Grumpy",
			"Sneaky",
			"Rude",
			"Enraged",
			"Insane",
			"Twisted",
			"Unwelcome",
			"Wicked"
		};
		
		public static string[] FunnyAdjectives = {

			"Goofy",
			"Wacky",
			"Lewd",
			"Silly",
			"Derpy",
			"Hyper",
			"Jittery",
			"Jumping",
			"Lustful",
			"Whimsical",
			"Horny",
			"Whiny"
		};
		
		public static string[] ColorAdjectives = {

			"Red",
			"Orange",
			"Yellow",
			"Green",
			"Teal",
			"Blue",
			"Purple",
			"Gold",
			"Silver",
			"Dark",
			"Light",
			"Black",
			"Grey",
			"White"

		};
		
		public static string[] GoodNouns = {

			"Victory",
			"Triumph",
			"Freedom",
			"Pride",
			"Success",
			"Accomplishment",
			"Zeal",
			"Devotion",
			"Motivation",
			"Vigor",
			"Benevolence",
			"Morality",
			"Love",
			"Light",
			"Ressurection",
			"Virtue",
			"Integrity",
			"Honor",
			"Dignity",
			"Purity",
			"Fortitude",
			"Discipline",
			"Redemption",
			"Advantage"
			
		};
		
		public static string[] NeutralNouns = {

			"Caution",
			"Discretion",
			"Guidance",
			"Persuasion",
			"Warning",
			"Stoicism",
			"Tolerance",
			"Resignation",
			"Vision",
			"Sacrifice",
			"Confusion",
			"Bewilderment",
			"Concern",
			"Aura",
			"Era",
			"Eon",
			"Epoch",
			"Climate"
			
		};
		
		public static string[] BadNouns = {

			"Malice",
			"Greed",
			"Wrath",
			"Gluttony",
			"Lust",
			"Spite",
			"Anger",
			"Envy",
			"Corruption",
			"Malevolence",
			"Revenge",
			"Animus",
			"Bitterness",
			"Grudge",
			"Hatred",
			"Hostility",
			"Loathing",
			"Belligerence",
			"Suffering",
			"Nightmare",
			"Darkness",
			"Plague",
			"Vice"
			
		};
		
		
		
		public static string[] FunnyNouns = {

			"Trickery",
			"Stench",
			"Aroma",
			"Left-Overs",
			"Dirty-Laundry",
			"Private-Stash"
			
		};
		
		public static string[] AuthorityNouns = {

			"Ambassador",
			"Diplomat",
			"Senator",
			"Dictator",
			"Autocrat",
			"Emperor",
			"Monarch",
			"Oligarch",
			"Advisor",
			"Politician",
			"Mayor",
			"President",
			"Minister",
			"Bishop",
			"Administrator"
		};
		
		public static string[] MilitaryNouns = {

			"Officer",
			"General",
			"Admiral",
			"Soldier",
			"Captain",
			"Private",
			"Commodore",
			"Commander",
			"Cadet",
			"Lieutenant",
			"Sergeant",
			"Major",
			"Corporal"
			
		};
		
		public static string[] BaddieNouns = {

			"Scoundrel",
			"Abductor",
			"Marauder",
			"Smuggler",
			"Murderer",
			"Thief",
			"Vagrant",
			"Aggressor",
			"Villain",
			"Thug",
			"Mugger",
			"Spy",
			"Infiltrator",
			"Violator",
			"Decimator",
			"Vandal",
			"Arsonist",
			"Saboteur",
			"Malcontent",
			"Interloper"
		};
		
		public static string[] ExplorerNouns = {

			"Explorer",
			"Voyager",
			"Wanderer",
			"Seeker",
			"Adventurer",
			"Nomad",
			"Scavenger",
			"Prospector",
			"Navigator"			
		};
		
		public static string[] JobNouns = {

			"Courier",
			"Mechanic",
			"Engineer",
			"Artist",
			"Merchant",
			"Programmer",
			"Scientist",
			"Chemist",
			"Biologist",
			"Doctor",
			"Nurse",
			"Janitor"
		};
		
		public static string[] BirdNouns = {

			"Pigeon",
			"Seagull",
			"Robin",
			"Starling",
			"Goose",
			"Chicken",
			"Turkey",
			"Osterich",
			"Duck",
			"Falcon",
			"Hawk",
			"Eagle",
			"Heron",
			"Vulture",
			"Woodpecker"
			
		};
		public static string[] AnimalNouns = {

			"Buffalo",
			"Rhinoceros",
			"Moose",
			"Stag",
			"Bear",
			"Snake",
			"Elephant",
			"Mammoth",
			"Tiger",
			"Lion",
			"Panther",
			"Cheetah",
			"Cougar",
			"Wolf",
			"Coyote",
			"Mule",
			"Mouse",
			"Badger",
			"Hound"
			
		};
		public static string[] FishNouns = {

			"Whale",
			"Shark",
			"Piranha",
			"Sword-Fish",
			"Octopus",
			"Squid",
			"Tuna",
			"Salmon",
			"Lobster",
			"Crab",
			"Mollusk",
			"Clam",
			"Barracuda",
			"Sturgeon",
			"Pickerel",
			"Eel",
			"Stingray",
			"Remora",
			"Manta",
			"Shrimp",
			"Prawn",
			"Koi",
			"Megalodon"

		};
		
		public static string[] InsectNouns = {

			"Bee",
			"Hornet",
			"Wasp",
			"Dragonfly",
			"Mantis",
			"Butterfly",
			"Moth",
			"Ant",
			"Cockroach",
			"Beetle",
			"Earwig",
			"Centipede",
			"Spider",
			"Scorpion",
			"Grasshopper",
			"Cricket",
			"Tarantula",
			"Mosquito"
			
		};
		
		public static string[] Surnames = {
			
			"Smith",
			"Johnson",
			"Williams",
			"Davidson",
			"Wilson",
			"Blanchard",
			"Gallant",
			"Moran",
			"Gonzalez",
			"Garrett",
			"Boudreau",
			"Cormier",
			"Parker",
			"Simpson",
			"Griffin",
			"Murphy",
			"Morrell",
			"Rogers",
			"Adams",
			"Van Luven",
			"Donnelley",
			"Allen",
			"Atkins",
			"Anderson",
			"Armstrong",
			"Bailey",
			"Banks",
			"Barnes",
			"Baxter",
			"Benson",
			"Masterson",
			"Massey",
			"Oakford",
			"Savoie",
			"Warner",
			"Compton",
			"Hill",
			"Henderson",
			"Paulsen",
			"Cobert",
			"Tillerman",
			"Samuels",
			"Jackson",
			"Lawrence",
			"Manning",
			"Hendricks",
			"Dunn",
			"Barton",
			"Steeves",
			"Wilkins",
			"Harding",
			"Langille",
			"Lancaster"
			
		};

		public static string CreateRandomNameFromPattern(string pattern) {

			string newPattern = pattern;

			if(newPattern.Contains("GoodAdjective") == true) {

				var randString = GoodAdjectives[Rnd.Next(0, GoodAdjectives.Length)];
				newPattern = newPattern.Replace("GoodAdjective", randString);

			}

			if(newPattern.Contains("NeutralAdjective") == true) {

				var randString = NeutralAdjectives[Rnd.Next(0, NeutralAdjectives.Length)];
				newPattern = newPattern.Replace("NeutralAdjective", randString);

			}

			if(newPattern.Contains("BadAdjective") == true) {

				var randString = BadAdjectives[Rnd.Next(0, BadAdjectives.Length)];
				newPattern = newPattern.Replace("BadAdjective", randString);

			}

			if(newPattern.Contains("FunnyAdjective") == true) {

				var randString = FunnyAdjectives[Rnd.Next(0, FunnyAdjectives.Length)];
				newPattern = newPattern.Replace("FunnyAdjective", randString);

			}

			if(newPattern.Contains("ColorAdjective") == true) {

				var randString = ColorAdjectives[Rnd.Next(0, ColorAdjectives.Length)];
				newPattern = newPattern.Replace("ColorAdjective", randString);

			}

			if(newPattern.Contains("GoodNoun") == true) {

				var randString = GoodNouns[Rnd.Next(0, GoodNouns.Length)];
				newPattern = newPattern.Replace("GoodNoun", randString);

			}

			if(newPattern.Contains("NeutralNoun") == true) {

				var randString = NeutralNouns[Rnd.Next(0, NeutralNouns.Length)];
				newPattern = newPattern.Replace("NeutralNoun", randString);

			}

			if(newPattern.Contains("BadNoun") == true) {

				var randString = BadNouns[Rnd.Next(0, BadNouns.Length)];
				newPattern = newPattern.Replace("BadNoun", randString);

			}

			if(newPattern.Contains("FunnyNoun") == true) {

				var randString = FunnyNouns[Rnd.Next(0, FunnyNouns.Length)];
				newPattern = newPattern.Replace("FunnyNoun", randString);

			}

			if(newPattern.Contains("AuthorityNoun") == true) {

				var randString = AuthorityNouns[Rnd.Next(0, AuthorityNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("AuthorityNoun's")) {

					newPattern = newPattern.Replace("AuthorityNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("AuthorityNoun", randString);

				}

			}

			if(newPattern.Contains("MilitaryNoun") == true) {

				var randString = MilitaryNouns[Rnd.Next(0, MilitaryNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("MilitaryNoun's")) {

					newPattern = newPattern.Replace("MilitaryNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("MilitaryNoun", randString);

				}

			}

			if(newPattern.Contains("BaddieNoun") == true) {

				var randString = BaddieNouns[Rnd.Next(0, BaddieNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("BaddieNoun's")) {

					newPattern = newPattern.Replace("BaddieNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("BaddieNoun", randString);

				}

			}

			if(newPattern.Contains("ExplorerNoun") == true) {

				var randString = ExplorerNouns[Rnd.Next(0, ExplorerNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("ExplorerNoun's")) {

					newPattern = newPattern.Replace("ExplorerNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("ExplorerNoun", randString);

				}

			}

			if(newPattern.Contains("JobNoun") == true) {

				var randString = JobNouns[Rnd.Next(0, JobNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("JobNoun's")) {

					newPattern = newPattern.Replace("JobNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("JobNoun", randString);

				}

			}

			if(newPattern.Contains("BirdNoun") == true) {

				var randString = BirdNouns[Rnd.Next(0, BirdNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("BirdNoun's")) {

					newPattern = newPattern.Replace("BirdNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("BirdNoun", randString);

				}

			}

			if(newPattern.Contains("AnimalNoun") == true) {

				var randString = AnimalNouns[Rnd.Next(0, AnimalNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("AnimalNoun's")) {

					newPattern = newPattern.Replace("AnimalNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("AnimalNoun", randString);

				}

			}

			if(newPattern.Contains("FishNoun") == true) {

				var randString = FishNouns[Rnd.Next(0, FishNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("FishNoun's")) {

					newPattern = newPattern.Replace("FishNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("FishNoun", randString);

				}

			}

			if(newPattern.Contains("InsectNoun") == true) {

				var randString = InsectNouns[Rnd.Next(0, InsectNouns.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("InsectNoun's")) {

					newPattern = newPattern.Replace("InsectNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("InsectNoun", randString);

				}

			}

			if(newPattern.Contains("SurnamesNoun") == true) {

				var randString = Surnames[Rnd.Next(0, Surnames.Length)];

				if(randString.EndsWith("s") && newPattern.Contains("SurnamesNoun's")) {

					newPattern = newPattern.Replace("SurnamesNoun's", randString + "'");

				} else {

					newPattern = newPattern.Replace("SurnamesNoun", randString);

				}

			}

			if(newPattern.Contains("RandomLetter") == true) {

				string randString = CharStringLetters[Rnd.Next(0, CharStringLetters.Length)].ToString();

				newPattern = ReplaceFirstOccurence("RandomLetter", randString, newPattern);

			}

			if(newPattern.Contains("RandomNumber") == true) {

				string randString = CharStringNumbers[Rnd.Next(0, CharStringNumbers.Length)].ToString();

				newPattern = ReplaceFirstOccurence("RandomNumber", randString, newPattern);

			}

			if(newPattern.Contains("RandomChar") == true) {

				string randString = CharStringAll[Rnd.Next(0, CharStringAll.Length)].ToString();

				newPattern = ReplaceFirstOccurence("RandomChar", randString, newPattern);

			}

			return newPattern;

		}

		//Following Method Found At This Source:
		//https://social.msdn.microsoft.com/Forums/en-US/25936e13-6ae6-4234-b604-d68d3c798d68/replace-only-first-instance-of-each?forum=regexp
		public static string ReplaceFirstOccurence(string wordToReplace, string replaceWith, string input) {

			Regex r = new Regex(wordToReplace, RegexOptions.IgnoreCase);

			return r.Replace(input, replaceWith, 1);

		}


	}

}