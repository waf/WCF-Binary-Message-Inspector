WCF Binary Message Inspector
============================

This is a modification of the [WCF Binary Message Inspector](http://archive.msdn.microsoft.com/wcfbinaryinspector). It provides two notable enhancements:

1. Uses a collapsible tree view instead of a textarea to display the decoded WCF binary message.
2. Allows editing and reissuing of captured WCF binary messages (thanks to [HofmaDresu](https://github.com/HofmaDresu))

Installation
------------

Copy the file `BinaryMessageFiddlerExtension/bin/Release/BinaryMessageFiddlerExtension.dll` 
([direct download](https://github.com/waf/WCF-Binary-Message-Inspector/raw/master/BinaryMessageFiddlerExtension/bin/Release/BinaryMessageFiddlerExtension.dll)) 
into `C:\Program Files\Fiddler2\Inspectors` and restart Fiddler. The inspector is titled "WCF Binary"

Tree View Usage
---------------

The tree view provides the following shortcuts:

* Ctrl+c — Copy current node XML
* Ctrl+Shift+c — Copy tree XML
* Alt+→ — Expand node and all child nodes
* Alt+← — Collapse node and all child nodes
* Middle-click — Toggle expand/collapse of node and all child nodes

Edit Usage
----------

1. Capture a WCF binary message
2. In the sessions list, right click on the message and choose "Unlock for editing"
3. Edit the request 
4. Reissue the request using the toolbar or the session list context menu

Development
-----------

The project is a Visual Studio 2010 project, and requires a reference to your `Fiddler.exe` file in order to compile.

See [Fiddler's page on Custom Inspectors](http://www.fiddler2.com/Fiddler/dev/Inspectors.asp) for more information.
