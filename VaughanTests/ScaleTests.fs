namespace VaughanTests
    module ScaleTests =
        open NUnit.Framework
        open FsCheck
        open FsCheck.NUnit
        open Swensen.Unquote
        open Vaughan.Domain
        open Vaughan.Scales
        open Vaughan.Chords

        [<Test>]
        let ``Should have notes for scales``() =
            createScaleNotes Ionian C =! [ C; D; E; F; G; A; B ]
            createScaleNotes Dorian C =! [ C; D; EFlat; F; G; A; BFlat ]
            createScaleNotes Phrygian C =! [ C; DFlat; EFlat; F; G; AFlat; BFlat ]
            createScaleNotes Lydian C =! [ C; D; E; FSharp; G; A; B ]
            createScaleNotes Mixolydian C =! [ C; D; E; F; G; A; BFlat ]
            createScaleNotes Aolian C =! [ C; D; EFlat; F; G; AFlat; BFlat ]
            createScaleNotes Locrian C =! [ C; DFlat; EFlat; F; GFlat; AFlat; BFlat ]
            createScaleNotes MajorPentatonic C =! [ C; D; E; G; A;]
            createScaleNotes MinorPentatonic C =! [ C; EFlat; F; G; BFlat ]
            createScaleNotes Blues C =! [ C; EFlat; F; GFlat; G; BFlat ]
            createScaleNotes HarmonicMinor C =! [ C; D; EFlat; F; G; AFlat; B ]
            createScaleNotes MelodicMinor C =! [ C; D; EFlat; F; G; A; B ]
            createScaleNotes Dorianb2 C =! [ C; DFlat; EFlat; F; G; A; BFlat ]
            createScaleNotes NeapolitanMinor C =! [ C; DFlat; EFlat; F; G; AFlat; B ]
            createScaleNotes LydianAugmented C =! [ C; D; E; FSharp; GSharp; A; B ]
            createScaleNotes LydianDominant C =! [ C; D; E; FSharp; G; A; BFlat ]
            createScaleNotes Mixolydianb6 C =! [ C; D; E; F; G; AFlat; BFlat ]
            createScaleNotes LocrianSharp2 C =! [ C; D; EFlat; F; GFlat; AFlat; BFlat ]
            createScaleNotes AlteredDominant C =! [ C; DFlat; DSharp; E; GFlat; GSharp; BFlat ]
            createScaleNotes HalfWholeDiminished C =! [ C; DFlat; EFlat; E; FSharp; G; A; BFlat ]
            createScaleNotes WholeTone C =! [ C; D; E; GFlat; GSharp; BFlat ]

        [<Property>]
        let ``It should return scales fitting a major triad`` (root :Note) =
            let chord = chord root ChordQuality.Major
            let chordNotes = chord.Notes |> List.map fst |> List.sort

            let scales = scalesFitting chord

            scales |> List.forall (
                fun s -> s.Notes |> List.filter (fun x -> (List.contains x chordNotes)) |> List.sort = chordNotes)

        [<Property>]
        let ``It should return scales fitting a minor triad`` (root :Note) =
            let chord = chord root ChordQuality.Minor
            let chordNotes = chord.Notes |> List.map fst |> List.sort

            let scales = scalesFitting chord

            scales |> List.forall (
                fun s -> s.Notes |> List.filter (fun x -> (List.contains x chordNotes)) |> List.sort = chordNotes)

        [<Property>]
        let ``It should return scales fitting a chord`` (root :Note) (quality: ChordQuality)=
            let chord = chord root quality
            let chordNotes = chord.Notes |> List.map fst |> List.sort

            let scales = scalesFitting chord

            scales |> List.forall (
                fun s -> s.Notes |> List.filter (fun x -> (List.contains x chordNotes)) |> List.sort = chordNotes)