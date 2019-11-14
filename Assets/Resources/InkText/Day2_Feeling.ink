//How are you feeling  
VAR PlayerFeeling = ""
Hey, how are you feeling? 
*   i'm fine
    you sure? 
    **  maybe i'm a bit too stressed
         ~PlayerFeeling = "Stressed"
        I had a feeling it was something
        *** it's nothing major -> HadAFeeling.Negative
    **  yeah why?
        something just feels a little off 
        *** with you?
            with you
            humour me, and feel your forehead
            is it warmer than normal? 
            ****    i can't really tell
                    maybe it's in my head
                    I can't begin to express how bizarre it is to just suddenly feel mortal. 
                    it's so uncomfortable
                *****   can I make it better 
                        no, no really here it's a privilege to feel mortal. 
                        it's just going to take some getting used to. 
                        you go so long here feeling only yourself to suddenly feel the warmth of another
                        it's nice
                        ******  hopefully we'll get to know each other more -> SetBoundaries
                *****   guess that means you're getting to know every inch of me
                        I can feel everytime you brush you hand in your hair. 
                        ******  can you feel this? -> SetBoundaries
*   great! -> HadAFeeling.Positive


=== SetBoundaries ===
-> DONE

=== HadAFeeling ===
= Positive 
-> DONE

= Negative
how exciting! 
*   exciting?
    yes! 
    I mean no
    you being {PlayerFeeling} is not the exciting part
    but that means i'm getting attuned to ever inch of you. 
    and that's exciting isn't it?! 
    **  is there a limit? -> LastFeeling.Limit

=== LastFeeling ===
=Exciting
yes! 
I mean no
you being {PlayerFeeling} is not the exciting part
but that means i'm getting attuned to ever inch of you. 
and that's exciting isn't it?! 
*[is there a limit?] -> Limit

= Limit
that's a good point
maybe we should set some boundaries
I'm sure it wouldn't be good for me to be commenting on every change you body goes through. 
*   Rule 1! Don't make it sound like a weird puberty thing
    okay okay
    being serious, I think it's a good idea to talk about this, I would hate to say something that makes you feel uncomfortable. 
    hmmm give me a few minutes.   
    -> DONE