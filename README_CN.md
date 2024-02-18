# SimpleEditorTableView

## 简介
SimpleEditorTableView 提供了一个简单的方法来创建 Unity Editor GUI 中的 TableView。
它封装了 Unity 的 MultiColumnHeader 和 ScrollView，但是接口更加简单和直接，
支持点击排序和自定义元素绘制。

完整 Demo 参见 [DemoWindow.cs](DemoWindow.cs)

![Screen](Screen.png)

## 开始使用

### 第一步：建立 Editor 类

1. 将 `SimpleEditorTableView.cs` 放到 Editor 目录下。
2. 定义一个数据结构，用于存储 TableView 中的数据。
3. 创建一个新的 EditorWindow 或者 Editor 类
4. 在你的 Editor 类中加入一个 `EditorTableView<TData>` 类的成员。

    ```csharp
        public class LightItem
        {
            public bool enabled;
            public string name;
            public LightType type;
            public LightShape shape;
            public Color color;
            public float intensity;
        }
    
    
        public class MyEditor : EditorWindow
        {
            private SimpleEditorTableView<LightItem> _tableView;
            
            
    
        }
    ```

### 第二步：定义表结构

调用 `SimpleEditorTableView<TData>.AddColumn()` 方法来定义 TableView 中的列。
调用者自己决定每个元素如何画控件，包括各种属性，如何排序等。

 ```csharp
      private void CreateTable()
      {
          _tableView = new SimpleEditorTableView<LightItem>();

          // first column is a toggle, with a minimum width of 50 and a maximum width of 80
          _tableView.AddColumn("Enabled", 50, (rect, item) =>
          {
              item.enabled = EditorGUI.Toggle(rect, item.enabled);
          }).SetMaxWidth(80);

          // second column is a text field, can be sorted by name
          _tableView.AddColumn("Name", 100, (rect, item) =>
          {
              item.name = EditorGUI.TextField(rect, item.name);
          }).SetMaxWidth(200).SetSorting((a, b) => a.name.CompareTo(b.name, StringComparison.Ordinal));

          // define more columns here
          // ...
     }
 ```

对于 `AddColumn` 方法，需要传入以下参数：
- title: 列的标题
- minWidth: 列的最小宽度
- onDrawItem: 一个委托，用于绘制每个元素的控件。它接受两个参数，一个是 Rect，表示控件的位置和大小，另一个是 TData，表示当前元素的数据。

`AddColumn` 返回值可以链式调用：
- SetMaxWidth: 设置列的最大宽度
- SetTooltip: 设置列的提示信息
- SetAutoResize: 设置列是否自动调整大小
- SetAllowToggleVisibility: 设置列是否可以隐藏
- SetSorting: 设置排序规则。传入一个比较器，用于比较两个元素的大小。只要实现升序排序即可，降序排序会自动处理。

### 第三步：在 OnGUI 中调用绘制
在 OnGUI 中调用 `SimpleEditorTableView<TData>.DrawTableGUI()` 方法来设置数据。
需要传入一个TData的数组。可以插入到 OnGUI 的任意位置。

 ```csharp
     private LightItem[] _lightItems = new LightItem[] {
         // ...
     }; 
     
     private void OnGUI()
     {
         // Draw some GUI here
         _tableView.DrawTableGUI(_lightItems);
         // Draw more GUI here
     }
 ```
