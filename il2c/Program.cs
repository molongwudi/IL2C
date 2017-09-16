﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace il2c
{
    class Test
    {
        public static int main()
        {
            var a = 1;
            var b = 2;
            var c = a + b;
            return c;
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var testType = typeof(Test);

            IL2C.Program.Main(testType);
        }
    }
}
