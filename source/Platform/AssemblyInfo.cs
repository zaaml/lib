// <copyright file="AssemblyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core;


[assembly: InternalsVisibleTo("Zaaml.UI,PublicKey=" +
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

[assembly: InternalsVisibleTo("Zaaml.PresentationCore,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]
