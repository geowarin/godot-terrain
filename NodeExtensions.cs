using System;
using System.Linq;
using Godot;

public static class NodeExtensions {
  public static T AddNodeIfNotExist<T>(this Node root, Func<T> create, string name) where T : Node {
    var child = root.GetChildren().OfType<T>().FirstOrDefault();

    if (child is null) {
      child = create();
      child.Name = name;
      root.AddChild(child);
      child.Owner = root.Owner;
    }

    return child;
  }
}
