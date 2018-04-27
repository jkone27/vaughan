﻿open Vaughan.Domain
open Vaughan.Chords
open Vaughan.Scales
open Vaughan.Guitar
open Vaughan.GuitarTab
open Vaughan.SpeechToMusic
open Vaughan.Infrastructure
open Vaughan.ChordVoiceLeading

open System
open Argu

type ChordShapes =
    | Closed = 0
    | Drop2 = 1
    | Drop3 = 2

type ChordInversions = 
    | Root = 0
    | First = 1
    | Second = 2
    | Third = 3

type BassStrings =
    | Third = 3
    | Fourth = 4
    | Fifth = 5
    | Sixth = 6

type VoiceLeadOptions =
    | Lead = 0
    | Bass = 1

type TabSubCommands =
    | Chord = 0
    | Scale = 1
    | Arpeggio = 2

type ScaleArguments =
    | [<AltCommandLine("-s")>]Scale of string
    | [<AltCommandLine("-min")>]MinFret of int
    | [<AltCommandLine("-max")>]MaxFret of int
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Scale _ -> "Specify a scale." 
            | MinFret _ -> "specify the minimum fret for the scale."
            | MaxFret _ -> "specify the maximum fret for the scale."

type ChordArguments =
    | [<AltCommandLine("-c")>]Chord of string
    | [<AltCommandLine("-b")>]Bass of BassStrings
    | [<AltCommandLine("-s")>]Shape of ChordShapes
    | [<AltCommandLine("-i")>]Inversion of ChordInversions
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chord _ -> "Specify a root note."
            | Shape _ -> "Specify a chord form."
            | Inversion _ -> "specify a chord inversion."
            | Bass _ -> "specify a guitar bass string for the chord."

type ChordsArguments =
    | [<AltCommandLine("-c")>]Chords of string list
    | [<AltCommandLine("-b")>]Bass of BassStrings
    | [<AltCommandLine("-s")>]Shape of ChordShapes 
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chords _ -> "Specify a list of chords."
            | Bass _ -> "specify a guitar bass string for the chords."
            | Shape _ -> "Specify a shape for the chords."

type VoiceLeadArguments =
    | Chords of ParseResults<ChordsArguments>
    | [<AltCommandLine("-b")>]Bass of BassStrings
    | [<AltCommandLine("-v")>]Voice of VoiceLeadOptions
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chords _ -> "Specify a list of chords."
            | Bass _ -> "specify a guitar bass string for the chords."
            | Voice _ -> "Specify a chord voice to use as lead."

type ArpeggioArguments =
    | [<AltCommandLine("-c")>]Chord of string
    | [<AltCommandLine("-min")>]MinFret of int
    | [<AltCommandLine("-max")>]MaxFret of int
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chord _ -> "Specify a chord." 
            | MinFret _ -> "specify the minimum fret for the arpeggio."
            | MaxFret _ -> "specify the maximum fret for the arpeggio."

type CommonScalesArguments =
    | [<AltCommandLine("-c")>]Chords of string list
    | [<AltCommandLine("-min")>]MinFret of int
    | [<AltCommandLine("-max")>]MaxFret of int
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chords _ -> "Specify a list of chords." 
            | MinFret _ -> "specify the minimum fret for the scale."
            | MaxFret _ -> "specify the maximum fret for the scale."

type ScalesForChordArguments =
    | [<AltCommandLine("-c")>]Chord of string
    | [<AltCommandLine("-min")>]MinFret of int
    | [<AltCommandLine("-max")>]MaxFret of int
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chord _ -> "Specify a chord." 
            | MinFret _ -> "specify the minimum fret for the scale."
            | MaxFret _ -> "specify the maximum fret for the scale."

type CLIArguments =
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-c")>]Chord of ParseResults<ChordArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-cs")>]Chords of ParseResults<ChordsArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-v")>]VoiceLead of ParseResults<VoiceLeadArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-s")>]Scale of ParseResults<ScaleArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-a")>]Arpeggio of ParseResults<ArpeggioArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-scs")>]CommonScales of ParseResults<CommonScalesArguments>
    | [<CliPrefix(CliPrefix.None)>][<AltCommandLine("-sc")>]ScalesForChord of ParseResults<ScalesForChordArguments>
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Chord _ -> "Specify a chord"
            | Chords _ -> "Specify a list of chords" 
            | VoiceLead _ -> "Specify a list of chords" 
            | Scale _ -> "Specify a scale"
            | Arpeggio _ -> "Specify an arpeggio"
            | CommonScales _ -> "Find common scales for a specified a list of chords" 
            | ScalesForChord _ -> "Find scales that contain all chord tones of a chord"

let handleGuitarString bass = 
    match bass with
    | BassStrings.Third -> ThirdString
    | BassStrings.Fourth -> FourthString
    | BassStrings.Fifth -> FifthString
    | BassStrings.Sixth -> SixthString
    | _ -> SixthString 

let handleChordShape shape =
    match shape with
    | ChordShapes.Closed -> toClosed
    | ChordShapes.Drop2 -> toDrop2
    | ChordShapes.Drop3 -> toDrop3
    | _ -> toClosed

let handleChordInversion (inversion:ChordInversions) = 
    match inversion with
    | ChordInversions.First -> invert
    | ChordInversions.Second -> invert >> invert
    | ChordInversions.Third -> invert >> invert >> invert
    | _ -> id

let handleVoiceLead voice =
    match voice with
    | VoiceLeadOptions.Lead -> lead
    | VoiceLeadOptions.Bass -> bass
    | _ -> lead

let handleError (e:ArguParseException) (parser:ArgumentParser<_>) subCommand =
    printf "%s" e.Message
    let parser = parser.GetSubCommandParser subCommand
    parser.PrintCommandLineSyntax(usageStringCharacterWidth = 20) |> printf "%s\n\n"
    Environment.Exit(-1) 

let handleChord (arguments:ParseResults<ChordArguments>) (parser:ArgumentParser<CLIArguments>) = 
    try
        let chordArguments = arguments.GetResult ChordArguments.Chord
        
        parseChord chordArguments
    with | :? ArguParseException as e ->
        handleError e parser Chord 
        chord C Major

let handleTabChord (arguments:ParseResults<ChordArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let bass = arguments.GetResult(ChordArguments.Bass, defaultValue = BassStrings.Sixth)
        let shape = arguments.GetResult(ChordArguments.Shape, defaultValue = ChordShapes.Closed)
        let inversion = arguments.GetResult(ChordArguments.Inversion, defaultValue = ChordInversions.Root)
        let chord = (handleChord arguments parser) 
                    |> handleChordShape shape 
                    |> (handleChordInversion inversion)

        Vaughan.Domain.Chord(guitarChord (handleGuitarString bass) chord)
    with | :? ArguParseException as e ->
        handleError e parser Chord
        Rest

let handleTabArpeggio (arguments:ParseResults<ArpeggioArguments>) (parser:ArgumentParser<CLIArguments>) = 
    try
        let chordArguments = arguments.GetResult ArpeggioArguments.Chord
        let minFret = arguments.GetResult(ArpeggioArguments.MinFret, defaultValue = 0)
        let maxFret = arguments.GetResult(ArpeggioArguments.MaxFret, defaultValue = 3)
        let chord = parseChord chordArguments

        Vaughan.Domain.Arpeggio(guitarArpeggio minFret maxFret chord)
    with | :? ArguParseException as e ->
        handleError e parser Arpeggio
        Rest

let handleTabScale (arguments:ParseResults<ScaleArguments>) (parser:ArgumentParser<CLIArguments>) = 
    try
        let scaleArguments = arguments.GetResult ScaleArguments.Scale
        let minFret = arguments.GetResult(ScaleArguments.MinFret, defaultValue = 0)
        let maxFret = arguments.GetResult(ScaleArguments.MaxFret, defaultValue = 3)

        let scale = parseScale scaleArguments 
        
        Vaughan.Domain.Scale(guitarScale minFret maxFret scale)
    with | :? ArguParseException as e ->
        handleError e parser Scale
        Rest

let handleChords (arguments:ParseResults<ChordsArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let shape = arguments.GetResult(ChordsArguments.Shape, defaultValue = ChordShapes.Closed)
        let chordNames = arguments.GetResult(ChordsArguments.Chords, defaultValue = [""])

        let toShape = handleChordShape shape

        chordNames
        |> List.map (parseChord >> toShape)

    with | :? ArguParseException as e ->
        handleError e parser Chords
        []

let handleTabChords (arguments:ParseResults<ChordsArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let bass = arguments.GetResult(ChordsArguments.Bass, defaultValue = BassStrings.Sixth)
        let toGuitarChord = guitarChord (handleGuitarString bass)
        let chords = handleChords arguments parser

        chords
        |> List.map (toGuitarChord >> Vaughan.Domain.Chord)

    with | :? ArguParseException as e ->
        handleError e parser Chords
        []

let handleTabVoiceLeadChords (arguments:ParseResults<VoiceLeadArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let bass = arguments.GetResult(VoiceLeadArguments.Bass, defaultValue = BassStrings.Sixth)
        let leadingVoice = arguments.GetResult(VoiceLeadArguments.Voice, defaultValue = VoiceLeadOptions.Lead) 
        let chordArguments = arguments.GetResult VoiceLeadArguments.Chords

        let chords = handleChords chordArguments parser
        let toGuitarChord = guitarChord (handleGuitarString bass)
        let voiceLeadStrategy = handleVoiceLead leadingVoice

        chords
        |> voiceLead voiceLeadStrategy 
        |> List.map (toGuitarChord >> Vaughan.Domain.Chord)

    with | :? ArguParseException as e ->
        handleError e parser Chords
        []

let handleTabScalesForChord (arguments:ParseResults<ScalesForChordArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let chordName = arguments.GetResult(ScalesForChordArguments.Chord, defaultValue = "")
        let minFret = arguments.GetResult(ScalesForChordArguments.MinFret, defaultValue = 0)
        let maxFret = arguments.GetResult(ScalesForChordArguments.MaxFret, defaultValue = 3)

        let scaleNamePair scale =
            scaleName scale.Scale, Vaughan.Domain.Scale scale

        chordName
        |> (parseChord >> scalesFitting)
        |> List.map (guitarScale minFret maxFret >> scaleNamePair)
        
    with | :? ArguParseException as e ->
        handleError e parser Chords
        [("", Rest)]

let handleTabCommonScales (arguments:ParseResults<CommonScalesArguments>) (parser:ArgumentParser<CLIArguments>) =
    try
        let chordNames = arguments.GetResult(CommonScalesArguments.Chords, defaultValue = [""])
        let minFret = arguments.GetResult(CommonScalesArguments.MinFret, defaultValue = 0)
        let maxFret = arguments.GetResult(CommonScalesArguments.MaxFret, defaultValue = 3)

        let printScaleName scaleName =
            printf "%s\n" scaleName
            scaleName

        chordNames
        |> List.map (parseChord >> scalesFitting)
        |> commonElements (fun scalesPerChord -> scalesPerChord |> List.map scaleName)
        |> Seq.fold (fun r s -> s::r) []
        |> Seq.toList
        |> List.map (printScaleName >> parseScale >> guitarScale minFret maxFret >> Vaughan.Domain.Scale)
        
    with | :? ArguParseException as e ->
        handleError e parser Chords
        []

let renderTab tab =
    [StandardTunning; Start] @ tab @ [End] |> renderTab |> printf "%s\n"

let handleTab (arguments:ParseResults<CLIArguments>) (parser:ArgumentParser<CLIArguments>) renderer =
    match arguments.GetSubCommand() with
    | Chord c -> renderer [handleTabChord c parser]
    | Chords cs -> renderer (handleTabChords cs parser)
    | VoiceLead v -> renderer (handleTabVoiceLeadChords v parser)
    | Arpeggio a -> renderer ([handleTabArpeggio a parser])
    | Scale s -> renderer ([handleTabScale s parser])
    | CommonScales s -> renderer (handleTabCommonScales s parser)
    | ScalesForChord sc -> (handleTabScalesForChord sc parser) |> List.iter (fun t -> printf "%s\n" (fst t); renderer [snd t])
    0

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<CLIArguments>(programName = "VaughanCLI", helpTextMessage = "Guitar tab CLI")
    
    try
        handleTab (parser.ParseCommandLine(argv)) parser renderTab
    with e ->
        printf "%s\n" e.Message
        0