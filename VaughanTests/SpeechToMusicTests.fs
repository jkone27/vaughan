﻿namespace VaughanTests
    module SpeechToMusicTests =
            open Xunit
            open FsUnit
            open FsUnit.Xunit
            open FsCheck
            open FsCheck.Xunit
            open System
            open Vaughan.Domain
            open Vaughan.Scales
            open Vaughan.SpeechToMusic
            open Vaughan.Chords
            open Vaughan.Guitar
            open Vaughan.GuitarTab
            open Vaughan.ScaleHarmonizer

            [<Fact>]
            let ``Should parse textual representation of triads``() =
                (parseChord "C Major") |> should equal { Root=C; Quality=Major }
                (parseChord "C# Maj") |> should equal { Root=CSharp; Quality=Major }
                (parseChord "C minor") |> should equal { Root=C; Quality=Minor }
                (parseChord "Db min") |> should equal { Root=DFlat; Quality=Minor }
                (parseChord "Cmin") |> should equal { Root=C; Quality=Minor }
                (parseChord "Cm") |> should equal { Root=C; Quality=Minor }
                (parseChord "C augmented") |> should equal { Root=C; Quality=Augmented }
                (parseChord "C Aug") |> should equal { Root=C; Quality=Augmented }
                (parseChord "C diminished") |> should equal { Root=C; Quality=Diminished }
                (parseChord "C dim") |> should equal { Root=C; Quality=Diminished }

            [<Fact>]
            let ``Should parse textual representation of seventh chords``() =
                (parseChord "C Major 7") |> should equal { Root=C; Quality=Major7 }
                (parseChord "C Maj 7") |> should equal { Root=C; Quality=Major7 }
                (parseChord "CMaj7") |> should equal { Root=C; Quality=Major7 }
                (parseChord "C minor 7") |> should equal { Root=C; Quality=Minor7 }
                (parseChord "C min 7") |> should equal { Root=C; Quality=Minor7 }
                (parseChord "Cmin7") |> should equal { Root=C; Quality=Minor7 }
                (parseChord "C augmented 7") |> should equal { Root=C; Quality=Augmented7 }
                (parseChord "C aug 7") |> should equal { Root=C; Quality=Augmented7 }
                (parseChord "C diminished 7") |> should equal { Root=C; Quality=Diminished7 }
                (parseChord "C dim 7") |> should equal { Root=C; Quality=Diminished7 }
                (parseChord "Caug7") |> should equal { Root=C; Quality=Augmented7 }
                (parseChord "Cdom7") |> should equal { Root=C; Quality=Dominant7 }
                (parseChord "C#7") |> should equal { Root=CSharp; Quality=Dominant7 }

            [<Fact>]
            let ``Should create chord from chord intent``() =
                let cIonian = createScaleNotes Ionian C
                let cMaj = triadsHarmonizer ScaleDegree.I cIonian
                let dMin = triadsHarmonizer ScaleDegree.II cIonian
                let eMin = triadsHarmonizer ScaleDegree.III cIonian
                let fMaj = triadsHarmonizer ScaleDegree.IV cIonian
                let gMaj = triadsHarmonizer ScaleDegree.V cIonian
                let aMin = triadsHarmonizer ScaleDegree.VI cIonian
                let bDim = triadsHarmonizer ScaleDegree.VII cIonian

                createChord { Root=C; Quality=Major } |> should equal (cMaj |> toClosed)
                createChord { Root=D; Quality=Minor } |> should equal (dMin |> toClosed)
                createChord { Root=E; Quality=Minor } |> should equal (eMin |> toClosed)
                createChord { Root=F; Quality=Major } |> should equal (fMaj |> toClosed)
                createChord { Root=G; Quality=Major } |> should equal (gMaj |> toClosed)
                createChord { Root=A; Quality=Minor } |> should equal (aMin |> toClosed)
                createChord { Root=B; Quality=Diminished } |> should equal (bDim |> toClosed)

            [<Fact>]
            let ``Should tabify chord from text``() =
                "A Major"
                |> parseChord
                |> createChord
                |> toTriad
                |> createGuitarChord SixthString
                |> tabify |> should equal ("      AMaj   " + Environment.NewLine+
                            "E||------------||" + Environment.NewLine +
                            "B||------------||" + Environment.NewLine +
                            "G||------------||" + Environment.NewLine +
                            "D||----2-------||" + Environment.NewLine +
                            "A||----4-------||" + Environment.NewLine +
                            "E||----5-------||" + Environment.NewLine)