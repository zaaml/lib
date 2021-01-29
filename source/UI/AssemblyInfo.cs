// <copyright file="AssemblyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Theming;
using DefaultMasterTheme = Zaaml.PresentationCore.Theming.DefaultMasterTheme;


[assembly: ThemeManagerBehavior]

[assembly: ThemeAssembly(typeof(DefaultMasterTheme), "Themes/Default")]


[assembly: InternalsVisibleTo("Zaaml.UI.Dev,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Artboard,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Docking,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Navigation,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Ribbon,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.PropertyView,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Spy,PublicKey=" +
                              ZaamlInfo.PublicKey)]