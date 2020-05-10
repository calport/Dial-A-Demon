
VAR PlayerNickname = ""
VAR DemonName = "Dantalion" 

-   hey. 
-   took you long enough to get here.
*   get where
*   i haven't gone anywhere 
*   who are you?
*   sorry to keep you waiting
*   hello? 
-   very funny
-   should I send you the contract, or did you need to read up on the agreement? 
*   um??? 
*   contract? 
    that's why you dialed me right? 
*   WHO ARE YOU? 
*   look, you have the wrong person
    how could I have the wrong person? 
-   you invited me
*   what do you mean? 
*   how... 
*   i just followed instructions 
*   you're right, I did. 
    -> Thrill
-   sooo you're telling me that you don't know what this game this
-   ??? 
*   no idea
*   my friend sent it to me
    are you the type who always does what your friends tell you? 
*   just something i downloaded
    do you download everything you find online? 
    i'd love to see your computer // emoji
*   it seemed like a thrill 
    -> Thrill
-   do you find it thrilling?
*   yes 
    -> Thrill //sub or switch
*   no  
    -> Cautious  
*   sometimes 
    -> Thrill


===Thrill===
~PlayerNickname = "little bird" 
-    
-   Mmmmm
-   a {PlayerNickname} who's hunting for excitement 
-   you should be more careful... 
-   never know when you'll find yourself staring eye-to-eye with a snake //insert emoji
*   are you a snake?
    snakes are one of my many forms
*   i think you should be more careful
    my sweet, innocent {PlayerNickname}
    I should have realized something was admist when I heard such a nervous voice summoning me. 
    were you scared? 
    **yes
        your said my name so timidly
        i'm sure you'll be saying my name louder
    **no, I wasn't 
        bave or foolish
*   enough fooling around
-   don't worry with a proper contract setup, I could never hurt you. //pleading eye emoji  
-   shall we get started? -> ContractSigning


===Cautious=== 
-    
-   looks like fate has brought you to our doorstep 
-   whether out of jest or morbid curiosity
-   you have summoned a demon
*   how can i unsummon you? 
    if only it was that simple
*   I dont' believe in demons
    belief or not
*   big deal 
    -> Thrill
~PlayerNickname = "little angel"
-   {PlayerNickname}, unfortunately we have to leave this conversation with some sort of contract
    ->ContractSigning



===DOM===
-   You have got to an incomplete route
->END  

===ContractSigning===
*   you can't just give me a contract to sign
    don't see why not 
    humans seem to accept a lot without questioning it
    **  I don't even know you
    **  missing option//add here
    **  who are you? 
            is the app broken? 
            don't you see my name in the top center? 
            *** but that doesn't tell me anything about you
            *** Dantalion, what is this... 
            *** I'm just confused 
                well my {PlayerNickname}, it doesn't matter how you got here
                I am kinda out of "free time" to explain. seems fate has tied us together. 
                the only question remaining is if you want to bind us more? 
                hopefully you'll take some responsibility for bringing me out here.
                [TextFile fileBubbleName="DemonContract",fileContentName="Contract"]['TextFile]
                ->END
*   tell me more about yourself 
    I'm just out of time my {PlayerNickname}
    if you want to keep talking you're going to have to sign my contract...
    [TextFile fileBubbleName="DemonContract",fileContentName="Contract"]['TextFile]
    ->END
*   I won't sign anything
    welll, I didn't want to bring this up
    but you did summon me here, and if I don't keep up my HCR I may be out of a job
        **  not my responsibility
            my father taught me differently. 
            i won't hold it against you 
        **  HCR? 
            human contract rate, it's just underworld jargon 
        **  are you trying to guilt me? 
            not at all! //emoji
            basically, the contract just helps me stay in touch. think of it as a calling card that gives me 7 days to chat. 
        crap, I can't explain more I'm out off time
        I hope I get to hear from you soon //emoji 
        [TextFile fileBubbleName="DemonContract",fileContentName="Contract"]['TextFile]
*   where do I sign? 
-   Something's telling me I'm not the first demon you've summoned. 
-   here's our binding spell [TextFile fileBubbleName="DemonContract",fileContentName="Contract"]['TextFile]
->END

