namespace Vaughan

    module Scales =
        open Domain
        open Notes
        open Chords

        type private ScalePattern = Interval list
        type private IScalePattern = ScaleType -> ScalePattern

        let private scalePattern:IScalePattern = function
            | Ionian -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; MajorSixth; MajorSeventh]
            | Dorian -> [Unisson; MajorSecond; MinorThird; PerfectFourth; PerfectFifth; MajorSixth; MinorSeventh]
            | Phrygian -> [Unisson; MinorSecond; MinorThird; PerfectFourth; PerfectFifth; MinorSixth; MinorSeventh]
            | Lydian -> [Unisson; MajorSecond; MajorThird; AugmentedFourth; PerfectFifth; MajorSixth; MajorSeventh]
            | Mixolydian -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; MajorSixth; MinorSeventh]
            | Aolian -> [Unisson; MajorSecond; MinorThird; PerfectFourth; PerfectFifth; MinorSixth; MinorSeventh]
            | Locrian -> [Unisson; MinorSecond; MinorThird; PerfectFourth; DiminishedFifth; MinorSixth; MinorSeventh]
            | MajorPentatonic -> [Unisson; MajorSecond; MajorThird; PerfectFifth; MajorSixth]
            | MinorPentatonic -> [Unisson; MinorThird; PerfectFourth; PerfectFifth; MinorSeventh]
            | Blues -> [Unisson; MinorThird; PerfectFourth; DiminishedFifth; PerfectFifth; MinorSeventh]
            | HarmonicMinor -> [Unisson; MajorSecond; MinorThird; PerfectFourth; PerfectFifth; MinorSixth; MajorSeventh]
            | MelodicMinor -> [Unisson; MajorSecond; MinorThird; PerfectFourth; PerfectFifth; MajorSixth; MajorSeventh]
            | Dorianb2 -> [Unisson; MinorSecond; MinorThird; PerfectFourth; PerfectFifth; MajorSixth; MinorSeventh]
            | NeapolitanMinor -> [Unisson; MinorSecond; MinorThird; PerfectFourth; PerfectFifth; MinorSixth; MajorSeventh]
            | LydianAugmented -> [Unisson; MajorSecond; MajorThird; AugmentedFourth; AugmentedFifth; MajorSixth; MajorSeventh]
            | LydianDominant -> [Unisson; MajorSecond; MajorThird; AugmentedFourth; PerfectFifth; MajorSixth; MinorSeventh]
            | Mixolydianb6 -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; MinorSixth; MinorSeventh]
            | Bebop -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; MajorSixth; MinorSeventh; MajorSeventh]
            | LocrianSharp2 -> [Unisson; MajorSecond; MinorThird; PerfectFourth; DiminishedFifth; MinorSixth; MinorSeventh]
            | AlteredDominant -> [Unisson; MinorSecond; AugmentedSecond; MajorThird; DiminishedFifth;  AugmentedFifth; MinorSeventh]
            | HalfWholeDiminished -> [Unisson; MinorSecond; MinorThird; MajorThird; AugmentedFourth;  PerfectFifth; MajorSixth; MinorSeventh]
            | WholeTone -> [Unisson; MajorSecond; MajorThird; DiminishedFifth; AugmentedFifth; MinorSeventh]
            | SixthDiminishedScale -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; AugmentedFifth; MajorSixth; MajorSeventh]
            | MinorSixthDiminishedScale -> [Unisson; MajorSecond; MinorThird; PerfectFourth; PerfectFifth; AugmentedFifth; MajorSixth; MajorSeventh]
            | DominantDiminishedScale -> [Unisson; MajorSecond; MajorThird; PerfectFourth; PerfectFifth; AugmentedFifth; MinorSeventh; MajorSeventh]
            | Dominantb5DiminishedScale -> [Unisson; MajorSecond; MajorThird; PerfectFourth; DiminishedFifth; AugmentedFifth; MinorSeventh; MajorSeventh]

        let createScale:ICreateScale = fun scale root ->
            scalePattern scale |> List.map (fun interval -> transpose root interval)

        let private createScaleProperties (scale:ScaleType) (root:Note) =
            {Scale=scale; Notes=createScale scale root};

        let private createAllScalesFrom (root:Note) =
            [
                createScaleProperties Ionian root;
                createScaleProperties Dorian root;
                createScaleProperties Phrygian root;
                createScaleProperties Lydian root;
                createScaleProperties Mixolydian root;
                createScaleProperties Aolian root;
                createScaleProperties Locrian root;
                createScaleProperties MajorPentatonic root;
                createScaleProperties MinorPentatonic root;
                createScaleProperties Blues root;
                createScaleProperties HarmonicMinor root;
                createScaleProperties MelodicMinor root;
                createScaleProperties Dorianb2 root;
                createScaleProperties NeapolitanMinor root;
                createScaleProperties LydianAugmented root;
                createScaleProperties LydianDominant root;
                createScaleProperties Mixolydianb6 root;
                createScaleProperties Bebop root;
                createScaleProperties LocrianSharp2 root;
                createScaleProperties AlteredDominant root;
                createScaleProperties HalfWholeDiminished root;
                createScaleProperties WholeTone root;
                createScaleProperties SixthDiminishedScale root;
                createScaleProperties MinorSixthDiminishedScale root;
                createScaleProperties DominantDiminishedScale root;
                createScaleProperties Dominantb5DiminishedScale root;
            ]

        let private scaleContainChordTones scale chordTones =
            ( scale |> List.filter 
                                    (fun x -> 
                                             (List.contains x chordTones)) 
                                             |> List.sort) = (chordTones |> List.sort)

        let scalesFitting (chord:Chord) =
            let chordTones = chord.Notes |> List.map fst

            createAllScalesFrom (root chord)
            |> List.choose (fun scale -> (if scaleContainChordTones scale.Notes chordTones then Some(scale) else None))