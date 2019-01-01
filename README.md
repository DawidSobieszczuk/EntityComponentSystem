# EntityComponentSystem

###### TextComponet.cs:
```c#
class TextComponent : ECS.Component {
	public string Text;
	public ConsoleColor Color;
}
```
###### WriteSystem.cs
```c#
class WriteSystem : ECS.System {
	public WriteSystem() : base(new[] { "TextComponent" }) {}

	public override void Draw() {
		Console.ForegroundColor = Get<TextComponent>().Color;
		Console.WriteLine(Get<TextComponent>().Text);
	}
}
```

###### Main.cs
```c#
static void Main(string[] args) {
	World world = new World();
	world.AddSystem(new WriteSystem());

	world.CreateEntity("R", new TextComponent() { Text = "Red", Color = ConsoleColor.Red });
	world.CreateEntity("G", new TextComponent() { Text = "Green", Color = ConsoleColor.Green });
	world.CreateEntity("B", new TextComponent() { Text = "Blue", Color = ConsoleColor.Blue });
	world.CreateEntity("Y", new TextComponent() { Text = "Yellow", Color = ConsoleColor.Yellow });
	world.CreateEntity("C", new TextComponent() { Text = "Cyan", Color = ConsoleColor.Cyan });

	world.Draw();

	Console.ReadKey();
}
```
