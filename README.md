# osu!design

Design-time UI creator for [osu!framework](https://github.com/ppy/osu-framework) based on a markup language.

## Requirements

- Refer to [osu!framework](https://github.com/ppy/osu-framework/blob/master/README.md)'s `Requirements` section.

## How to use

osu!design is not ready for any real project due to the lack of features and mostly untested code. There are no releases either.

However, you can try the application by launching it through an IDE. [Visual Studio Code](https://code.visualstudio.com/) (with the [C# plugin](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) installed) is recommended.

Once the application is running, you can browse to the `osuML` [directory](osuML) and mess around with the example markups provided. They have the extension `.osuml`.

## osuML

`osuML` is an XML-based markup language that is used to compose a `Drawable`. It stands for _osu! Markup Language_. It is **not XAML** and does not intend to be one.

Every element in `osuML` represents a `Drawable` node, and their attributes correspond to a field or property.

```xml
<Container
    xmlns="osufx://osu.Framework/*"
    Name="MyDrawable"
    Size="400,300">
    <Box
        Name="Background"
        RelativeSizeAxes="Both"
        Colour="rgba(250,150,100,200)"/>
    <Container
        Name="MyContainer"
        Padding="60,40,60,40"
        RelativeSizeAxes="Both">
        <Box
            RelativeSizeAxes="Both"
            Colour="#00ff00, #ff00ff, #00ffff, #ff0000"/>
    </Container>
    <SpriteText
        Text="My SpriteText is awesome"
        TextSize="20"
        Margin="10,10,10,10"/>
</Container>
```

Breaking down the above markup:

- The root node is a `Container`. When the designer generates a class from this markup, it will be derived from `Container`.
- `xmlns` attribute specifies the default namespace from which `Drawable` types are imported. The URI must have the `osufx://` scheme, followed by the name of the assembly without `.dll` extension, then a glob pattern to filter the types.
  In this example, `osufx://osu.Framework/*` imports all `Drawable` types from the assembly `osu.Framework.dll`, which match the glob pattern `*` (everything).
- `Name` attribute specifies the given name of the `Drawable`. This is required for the root node but optional for any of its descendants. Refer to the [ECMA C# spec](https://www.ecma-international.org/publications/files/ECMA-ST-ARCH/ECMA-334%201st%20edition%20December%202001.pdf) for what is considered a valid class name. TLDR: your usual alphanumeric identifier with underscores and no spaces.
  In this example, the `Name` of the root node is `MyDrawable` and therefore the generated class will be named after it.
- `Size` attribute specifies the size of the `Drawable`, in this case, in absolute measurement. (Add `RelativeSizeAxes="Both"` to make it measure relatively.)

```xml
<Box
    Name="Background"
    RelativeSizeAxes="Both"
    Colour="rgba(250,150,100,200)"/>
```

- The root node has a child element `Box` named `Background`, relatively sized in both axes. `Colour` attribute specifies the colour of the `Box`.
  You can specify the colour as an RGB function (`rgb(r,g,b)`, `rgba(r,g,b,a)`) or a hexadecimal (`#012345`). A single colour can be specified for all four vertices of the `Drawable`, or a comma-separated list of colours for each individual vertex in the order of top-left, top-right, bottom-right, bottom-left.

```xml
<Container
    Name="MyContainer"
    Padding="60,40,60,40"
    RelativeSizeAxes="Both">
    <Box
        RelativeSizeAxes="Both"
        Colour="#00ff00, #ff00ff, #00ffff, #ff0000"/>
```

- The root node has a child element `Container` named `MyContainer`, relatively sized in both axes.
  It has a padding of `60`, `40`, `60` and `40` for the top, right, bottom and left respectively.
- `MyContainer` has one unnamed child element `Box`, relatively sized in both axes and has the colours `#00ff00`, `#ff00ff`, `#00ffff` and `#ff0000` for its four vertices.

```xml
<SpriteText
    Text="My SpriteText is awesome"
    TextSize="20"
    Margin="10,20,10,20"/>
```

- Lastly, the root node has an unnamed child element `SpriteText`. It has the text `My SpriteText is awesome`, is sized at `20` pixels and has a margin of `10`, `20`, `10` and `20` for the top, right, bottom and left respectively.

More examples can be found in the [osuML](osuML) folder.

## To-dos

At the moment, osu!design is lacking in some of the most important and useful features of osu!framework such as:

- Resource management
- Texture resource binding for `Sprite`s
- Config-backed binding of properties
- Local-scope bidirectional binding between properties
- Asynchronous loading of children `Drawable`s
- XML-based transform sequence generation
- XML-based `ConfigManager<T>` generation

## Licence

This project is licensed under the [MIT licence](https://opensource.org/licenses/MIT). Please see [the licence file](LICENCE) for more information. [tl;dr](https://tldrlegal.com/license/mit-license) you can do whatever you want as long as you include the original copyright and license notice in any copy of the software/source.

The BASS audio library (a dependency of [osu!framework](https://github.com/ppy/osu-framework)) is a commercial product. While it is free for non-commercial use, please ensure to [obtain a valid licence](http://www.un4seen.com/bass.html#license) if you plan on distributing any application using it commercially.
