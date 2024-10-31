/* 
* FILE : gameEngine.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used to load a random  game file into the game. It 
* includes values needed to play the game - random 80 char string, word count, and word list
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace A06_Server
{
    internal class gameEngine
    {

        // FUNCTION    : loadWords
        // DESCRIPTION : This function chooses a random game file and loads the words from
        //               the file 
        // PARAMETERS  : Nothing
        // RETURNS     : lines
        public static string[] loadWords()
        {
            // instantiate random
            Random rnd = new Random();
            // random value between 1 - 3
            int randNum = rnd.Next(1, 4);

            string gameFile = ConfigurationManager.AppSettings["GameFiles"];
            // configurable file path   
            string filePath = gameFile + randNum + ".txt";

            // load all lines int variable
            var lines = File.ReadAllLines(filePath);
            // return the file contents
            return lines;
        }
    }
}
