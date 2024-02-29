// <copyright file="ExternalInternalsVisibleAttributes.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core;

[assembly: InternalsVisibleTo("Zaaml.Core.Test,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Text.Test,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.PresentationCore.Test,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Test,PublicKey=" +
                              ZaamlInfo.PublicKey)]
