using RedditSharp;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.RegularExpressions;

namespace CommentCombobulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferWidth = 100;//Make console wider for better reading
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



            Post p = reddit.GetPost("http://www.reddit.com/r/AskReddit/comments/1orzia/people_who_had_an_affair_while_in_a_relationship/");//Put your post url (with http://) here
            Comment[] comments = p.GetComments();

            Console.WriteLine("Got post comments");

            string output = string.Empty;

            int i = 0;
            foreach (Comment c in comments)
            {
                try
                {
                    string text = c.Body;
                    string[] sentences = text.Split('.');

                    string sentence = sentences[MaxAt(i, sentences.Length - 1)] + ".";

                    if (sentence.Contains("EDIT") | sentence.Contains("[deleted]") | sentence.Contains("Edit"))
                    {
                        sentence = "";
                    }
                    sentence.Trim();

                    Regex whitespaceremoval = new Regex(@"\s\s+");

                    sentence = whitespaceremoval.Replace(sentence,"");

                    Regex alphanum = new Regex("[^a-zA-Z0-9. -]");
                    sentence = alphanum.Replace(sentence, "");


                    output += sentence;

                    output.Trim();
                    i++;

                    Console.WriteLine("Parsed comment:" + i);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to parse comment :" + e.Message);//Error catching for that damn null comment at the end of every post
                }
            }
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(output);
            Console.ReadKey();
        }
        static int MaxAt(int input, int maxlength)
        {
            if(input > maxlength)
            {
                return maxlength;
            }
            return input;
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
