using RedditSharp;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace CommentCombobulator
{
    class Program
    {
        



        static void Main(string[] args)
        {
            Console.BufferWidth = 100;//Make console wider for better reading
            Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);
            Reddit reddit = new Reddit();
            while (reddit.User == null)//Keep trying to login until it works
            {
                Console.Write("Username: ");
                var username = Console.ReadLine();
                Console.Write("Password: ");
                var password = ReadPassword();
                try
                {
                    Console.WriteLine("Logging in...");
                    reddit.LogIn(username, password);
                }
                catch (AuthenticationException)
                {
                    Console.WriteLine("Incorrect login.");
                }
            }
            Console.WriteLine("Logged in.");



            Post p = reddit.GetPost("http://www.reddit.com/r/AskReddit/comments/1m15pa/teachers_of_reddit_what_is_the_funniest_thing/");//Put your post url (with http://) here
            Comment[] comments = p.GetComments();

            Console.WriteLine("Got post comments");

            string output = string.Empty;

            int i = 0;
            foreach (Comment c in comments)
            {
                try
                {
                    string text = c.Body;//Take the text of the string from the comment

                    if (!ContainsBlackListedWords(text))//Make sure the comment isn't a witty one liner (rudimentary)
                    {

                        string[] sentences = text.Split('.');//Split the comment into each setence

                        string sentence = sentences[i.MaxAt(sentences.Length-1)] + ".";//Take a sentence out of the sentence array
                        if (sentence.Length > 2 )//Check if it is a good sentence
                        {
                            sentence.Trim();//Trim spaces from both sides of the sentence.

                            Regex whitespaceremoval = new Regex(@"\s\s+");

                            sentence = whitespaceremoval.Replace(sentence, "");//Apply regex to remove excess spacing inside the sentence (readability)

                            //Regex alphanum = new Regex("[^a-zA-Z0-9. ,-]");

                            //sentence = alphanum.Replace(sentence, "");//Remove crazy symbol using regex (readability)


                            output += sentence;//Add the sentence to the output string

                            i++;//Increment i to continue the paragraph

                            Console.WriteLine("Parsed comment:" + i);//Log success
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse comment : Comment contained blacklisted word");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to parse comment :" + e.Message);//Error catching for that damn null comment at the end of every post
                }
            }
            Console.WriteLine();//Add spacing
            Console.WriteLine();//""


            Console.Write(output);

            
            File.WriteAllText("out" + DateTime.Now.Ticks.ToString() + ".txt", output);



            Console.ReadKey();
        }
        static bool ContainsBlackListedWords(string sample)
        {
            string[] blacklistedwords = { "jpeg", "jpg", "png", "edit", "com", "www", "gold"};//A list of words that, if found in a comment, will skip the comment from being used

            

            foreach (string s in blacklistedwords)
            {
                if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(sample, s,CompareOptions.IgnoreCase) != -1)
                {
                    return true;
                }
            }
            return false;


        }
        public static string ReadPassword()//Stolen from the RedditSharp example
        {
            var passbits = new Stack<string>();
            //keep reading
            for (ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey(true))
            {
                if (cki.Key == ConsoleKey.Backspace)
                {
                    //rollback the cursor and write a space so it looks backspaced to the user
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    passbits.Pop();
                }
                else
                {
                    Console.Write("*");
                    passbits.Push(cki.KeyChar.ToString());
                }
            }
            string[] pass = passbits.ToArray();
            Array.Reverse(pass);
            Console.Write(Environment.NewLine);
            return string.Join(string.Empty, pass);
        }
    }
}
