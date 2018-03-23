#!/usr/bin/env python3
import json
import glob
import os
import re

script_dir = os.path.dirname(os.path.abspath(__file__))
output_dir = os.path.join(script_dir, "..")

def to_csharp_name(name):
    return "".join(x for x in name.title() if not x.isspace()).replace("-", "").replace("(", "").replace(")", "").replace("~", "")

def raw_literal(s):
    return '@"' + s.replace('"', '""') + '"'

def gen_new_anonymous(o):
    switch_dict = None

    def on_bool(v):
        return "true" if v else "false"

    def on_int(v):
        return str(v)

    def on_float(v):
        return str(v)

    def on_string(v):
        return raw_literal(v)

    def on_dict(v):
        return gen_new_anonymous(v)

    def on_list(v):
        r = "new object[] {"
        for x in v:
            r += switch_dict[type(x)](x) + ", "
        r += "}"
        return r

    def on_lambda(v):
        return v["csharp"]

    switch_dict = {
        bool: on_bool, 
        int: on_int,
        float: on_float,
        str: on_string,
        dict: on_dict,
        list: on_list
    }

    ret = "new {"
    for k in o:
        if k == "lambda" and o["lambda"]["csharp"]:
            ret += k + " = " + on_lambda(o[k]) + ", "
        else:
            ret += k + " = " + switch_dict[type(o[k])](o[k]) + ", "
    ret += "}"
    return ret

def dirty_fix(spec):
    spec["name"] = os.path.split(path[:-5])[1]

    field_map = {
        "string" : "string_",
        "bool" : "bool_",
        "static" : "static_",
    }

    def rec(o):
        for k, v in field_map.items():
            if k in o:
                o[v] = o[k]
                del o[k]

        for k in o:
            if type(o[k]) == dict:
                rec(o[k])

    template_map = {
        "{{string}}" : "{{string_}}",
        "{{&string}}" : "{{&string_}}",
        "{{ string }}" : "{{ string_ }}",
        "{{& string }}" : "{{& string_ }}",
        "{{bool}}" : "{{bool_}}",
        "{{^bool}}" : "{{^bool_}}",
        "{{/bool}}" : "{{/bool_}}",
        "{{#bool}}" : "{{#bool_}}",
        "{{static}}" : "{{static_}}",
    }

    for test in spec["tests"]:
        t = test["template"]
        for k, v in template_map.items():
            if k in t:
                t = t.replace(k, v)
        test["template"] = t
        rec(test["data"])

specs = []

for path in glob.glob(os.path.join(script_dir, "specs", "*.json")):
    with open(path) as f:
        obj = json.load(f)
        dirty_fix(obj)
        specs.append(obj)

for spec in specs:
    class_name = to_csharp_name(spec["name"]) + "Spec"
    fname = class_name + ".cs"

    with open(os.path.join(output_dir, fname), "w") as f:
        f.write("using System;\n")
        f.write("using System.Collections.Generic;\n")
        f.write("using Mustache;\n")
        f.write("using Xunit;\n")
        f.write("\n")
        f.write("namespace Mustache.Test {\n")
        f.write("public class " + class_name + " {\n")
        f.write("/*\n")
        f.write(spec["name"] + "\n\n")
        f.write(spec["overview"])
        f.write("*/\n")
        for test in spec["tests"]:
            test_name = to_csharp_name(spec["name"] + " " + test["name"])
            template = raw_literal(test["template"])
            expected = raw_literal(test["expected"])
            desc = raw_literal(test["desc"])
            f.write("\n")
            f.write("[Fact]\n")
            f.write("public void Test" + test_name + "() { \n")
            if test["data"]:
                if test["name"] == "Interpolation - Multiple Calls":
                    f.write("    int calls = 0;\n") # :{
                test_data_name = test_name + "Data"
                f.write("    object data = ")
                f.write(gen_new_anonymous(test["data"]))
                f.write(";\n")
            else:
                f.write("    object data = null;\n")
            if "partials" in test:
                f.write("    var partials = new Dictionary<string, string>() {\n");
                for key, value in test["partials"].items():
                    f.write("        {" + raw_literal(key) + ", " + raw_literal(value) + "},\n")
                f.write("    };\n")
            else:
                f.write("    Dictionary<string, string> partials = null;\n")
            f.write("    var template = " + template + ";\n")
            f.write("    var expected = " + expected + ";\n")
            f.write("    var actual = new MustacheRenderer().Render(template, data, partials);\n")
            f.write("    Assert.Equal(expected, actual);\n")
            f.write("}\n")
        f.write("\n")
        f.write("} // class\n")
        f.write("} // namespace\n")
