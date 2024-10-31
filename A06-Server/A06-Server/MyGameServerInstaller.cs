/* 
* FILE : MyGameServerInstaller.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used initialize mygameserverinstaller
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace A06_Server
{
    [RunInstaller(true)]
    public partial class MyGameServerInstaller : System.Configuration.Install.Installer
    {
        public MyGameServerInstaller()
        {
            InitializeComponent();
        }
    }
}
