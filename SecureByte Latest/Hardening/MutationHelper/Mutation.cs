internal class MutationClass
{
    public static readonly int KeyI0 = 0;
    public static readonly int KeyI1 = 1;
    public static readonly int KeyI2 = 2;
    public static readonly int KeyI3 = 3;
    public static readonly int KeyI4 = 4;
    public static readonly int KeyI5 = 5;
    public static readonly int KeyI6 = 6;
    public static readonly int KeyI7 = 7;
    public static readonly int KeyI8 = 8;
    public static readonly int KeyI9 = 9;
    public static readonly int KeyI10 = 10;
    public static readonly int KeyI11 = 11;
    public static readonly int KeyI12 = 12;
    public static readonly int KeyI13 = 13;
    public static readonly int KeyI14 = 14;
    public static readonly int KeyI15 = 15;
    public static readonly string Str1 = "a";
    public static readonly string Str2 = "b";
    public static readonly string Str3 = "c";
    public static readonly string Str4 = "d";
    public static readonly string Str5 = "e";
    public static readonly string Str6 = "f";
    public static readonly string Str7 = "g";
    public static readonly string Str8 = "h";
    public static readonly string Str9 = "i";
    public static readonly string Str10 = "j";
    public static readonly string Str11 = "k";
    public static readonly string Str12 = "l";
    public static readonly string Str13 = "m";
    public static readonly string Str14 = "n"; //rtx86
    public static readonly string Str15 = "o"; //rtx64
    public static readonly string Str16 = "p";
    public static T Placeholder<T>(T val)
    {
        return val;
    }
    public static T Value<T>()
    {
        return default(T);
    }
    public static T Value<T, Arg0>(Arg0 arg0)
    {
        return default(T);
    }
    public static void Crypt(uint[] data, uint[] key) { }
}
