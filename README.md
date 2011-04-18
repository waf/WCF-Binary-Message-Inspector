WCF Binary Message Inspector
============================

This is a modification of the [WCF Binary Message Inspector](http://archive.msdn.microsoft.com/wcfbinaryinspector) that uses a treeview instead of a textarea.

Installation
------------

Copy the file `BinaryMessageFiddlerExtension/bin/Release/BinaryMessageFiddlerExtension.dll` into `C:\Program Files\Fiddler2\Inspectors` and restart Fiddler.

Usage
-----

This inspector provides the following shortcuts:

* Ctrl+c — Copy current node XML
* Ctrl+Shift+c — Copy tree XML
* Alt+→ — Expand node and all child nodes
* Alt+← — Collapse node and all child nodes
* Middle-click — Toggle expand/collapse of node and all child nodes

Development
-----------

The project is a Visual Studio 2010 project, and requires a reference to your `Fiddler.exe` file in order to compile.

See [Fiddler's page on Custom Inspectors](http://www.fiddler2.com/Fiddler/dev/Inspectors.asp) for more information.
