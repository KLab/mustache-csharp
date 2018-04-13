using System;
using System.Collections.Generic;
using KLab.Mustache;
using Xunit;

namespace MustacheTest {
public class CommentsSpec {
/*
comments

Comment tags represent content that should never appear in the resulting
output.

The tag's content may contain any substring (including newlines) EXCEPT the
closing delimiter.

Comment tags SHOULD be treated as standalone when appropriate.
*/

[Fact]
public void TestCommentsInline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"12345{{! Comment Block! }}67890";
    var expected = @"1234567890";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsMultiline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"12345{{!
  This is a
  multi-line comment...
}}67890
";
    var expected = @"1234567890
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsStandalone() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
{{! Comment Block! }}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsIndentedStandalone() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
  {{! Indented Comment Block! }}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsStandaloneLineEndings() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"|
{{! Standalone Comment }}
|";
    var expected = @"|
|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsStandaloneWithoutPreviousLine() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"  {{! I'm Still Standalone }}
!";
    var expected = @"!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsStandaloneWithoutNewline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"!
  {{! I'm Still Standalone }}";
    var expected = @"!
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsMultilineStandalone() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
{{!
Something's going on here...
}}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsIndentedMultilineStandalone() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
  {{!
    Something's going on here...
  }}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsIndentedInline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"  12 {{! 34 }}
";
    var expected = @"  12 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestCommentsSurroundingWhitespace() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"12345 {{! Comment Block! }} 67890";
    var expected = @"12345  67890";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
