﻿// <copyright file="GrammarBuilders.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// *************************************************************
//
// AutoGenerated
//
// *************************************************************

// ReSharper disable All

namespace Zaaml.Text
{
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, out TResult>(TSyntaxFactory factory);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, out TResult>(TSyntaxFactory factory, TArg1 arg1);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, in TArg28, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27, TArg28 arg28);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, in TArg28, in TArg29, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27, TArg28 arg28, TArg29 arg29);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, in TArg28, in TArg29, in TArg30, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27, TArg28 arg28, TArg29 arg29, TArg30 arg30);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, in TArg28, in TArg29, in TArg30, in TArg31, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27, TArg28 arg28, TArg29 arg29, TArg30 arg30, TArg31 arg31);
	public delegate TResult SyntaxBindFunc<in TSyntaxFactory, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, in TArg7, in TArg8, in TArg9, in TArg10, in TArg11, in TArg12, in TArg13, in TArg14, in TArg15, in TArg16, in TArg17, in TArg18, in TArg19, in TArg20, in TArg21, in TArg22, in TArg23, in TArg24, in TArg25, in TArg26, in TArg27, in TArg28, in TArg29, in TArg30, in TArg31, in TArg32, out TResult>(TSyntaxFactory factory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, TArg17 arg17, TArg18 arg18, TArg19 arg19, TArg20 arg20, TArg21 arg21, TArg22 arg22, TArg23 arg23, TArg24 arg24, TArg25 arg25, TArg26 arg26, TArg27 arg27, TArg28 arg28, TArg29 arg29, TArg30 arg30, TArg31 arg31, TArg32 arg32);

	internal abstract partial class Grammar<TGrammar, TToken, TNode, TSyntaxFactory>
	{
		protected internal sealed partial class ParserSyntaxNode<TActualNode>
		{
			public void BindFactory<TResult>(SyntaxBindFunc<TSyntaxFactory, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}
			public void BindFactory<TArg1, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TArg31, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TArg31, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
			public void BindFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TArg31, TArg32, TResult>(SyntaxBindFunc<TSyntaxFactory, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16, TArg17, TArg18, TArg19, TArg20, TArg21, TArg22, TArg23, TArg24, TArg25, TArg26, TArg27, TArg28, TArg29, TArg30, TArg31, TArg32, TResult> expression, ParserSyntaxProduction parserProduction)
			{
				parserProduction.ProductionBinding = SyntaxFactoryBinding.Bind<TNode, TSyntaxFactory>(expression);

				AddProductionCore(parserProduction);
			}			
		}
	}
}