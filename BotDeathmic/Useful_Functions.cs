﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathmicChatbot
{
    static class Useful_Functions
    {
        public static String[] String_Array_Push(String[] _input,string item)
        {
            List<string> temp = _input.ToList();
            temp.Add(item);
            return temp.ToArray();
        }



    }
}