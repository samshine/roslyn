﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Completion.Providers;
using Microsoft.CodeAnalysis.CSharp.Completion.Providers;
using Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Completion.CompletionProviders;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Completion.CompletionSetSources
{
    public class AttributeNamedParameterCompletionProviderTests : AbstractCSharpCompletionProviderTests
    {
        internal override ICompletionProvider CreateCompletionProvider()
        {
            return new AttributeNamedParameterCompletionProvider();
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void SendEnterThroughToEditorTest()
        {
            VerifySendEnterThroughToEnter("Foo", "Foo", sendThroughEnterEnabled: false, expected: false);
            VerifySendEnterThroughToEnter("Foo", "Foo", sendThroughEnterEnabled: true, expected: true);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void CommitCharacterTest()
        {
            TestCommonIsCommitCharacter();
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void SimpleAttributeUsage()
        {
            var markup = @"
using System;
class class1
{
    [Test($$
    public void Foo()
    {
    }
}
 
public class TestAttribute : Attribute
{
    public ConsoleColor Color { get; set; }
}";

            VerifyItemExists(markup, "Color =");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AfterComma()
        {
            var markup = @"
using System;
class class1
{
    [Test(Color = ConsoleColor.Black, $$
    public void Foo()
    {
    }
}

public class TestAttribute : Attribute
{
    public ConsoleColor Color { get; set; }
    public string Text { get; set; }
}";

            VerifyItemExists(markup, "Text =");
        }

        [WorkItem(544345)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ExistingItemsAreFiltered()
        {
            var markup = @"
using System;
class class1
{
    [Test(Color = ConsoleColor.Black, $$
    public void Foo()
    {
    }
}

public class TestAttribute : Attribute
{
    public ConsoleColor Color { get; set; }
    public string Text { get; set; }
}";

            VerifyItemExists(markup, "Text =");
            VerifyItemIsAbsent(markup, "Color =");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AttributeConstructor()
        {
            var markup = @"
using System;
class TestAttribute : Attribute
{
    public TestAttribute(int a = 42)
    { }
}
 
[Test($$
class Foo
{ }
";

            VerifyItemExists(markup, "a:");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AttributeConstructorAfterComma()
        {
            var markup = @"
using System;
class TestAttribute : Attribute
{
    public TestAttribute(int a = 42, string s = """")
    { }
}

[Test(s:"""", $$
class Foo
{ }
";

            VerifyItemExists(markup, "a:");
        }

        [WorkItem(545426)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void TestPropertiesInScript()
        {
            var markup = @"
using System;

class TestAttribute : Attribute
{
    public string Text { get; set; }
    public TestAttribute(int number = 42)
    {
    }
}
 
[Test($$
class Foo
{
}";

            VerifyItemExists(markup, "Text =");
        }

        [WorkItem(1075278)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void NotInComment()
        {
            var markup = @"
using System;
class class1
{
    [Test( //$$
    public void Foo()
    {
    }
}
 
public class TestAttribute : Attribute
{
    public ConsoleColor Color { get; set; }
}";

            VerifyNoItemsExist(markup);
        }
    }
}
