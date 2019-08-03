
using CharFA = Pck.CharFA<string>;
class Program
{
	static void Main(string[] args)
	{
		var foo = new CharFA();
		var foo2 = new CharFA();
		var foo3 = new CharFA();
		var foo4 = new CharFA();
		foo.Transitions.Add('f', foo2);
		foo2.Transitions.Add('o', foo3);
		foo3.Transitions.Add('o', foo4);
		foo4.IsAccepting = true;
		foo4.EpsilonTransitions.Add(foo3);
		foo.EpsilonTransitions.Add(foo4);
		foo = foo.ToDfa(); // added this line
		foo.RenderToFile(@"..\..\..\foo.jpg");
		return;
	}		
}

