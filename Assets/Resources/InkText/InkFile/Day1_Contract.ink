VAR PlayerNickname = "" 
VAR DemonName = "Dantalian" 
-   hey. <\\TextFile fileBubbleName=\"DemonContract\",fileContentName=\"fileContentName\"><\\TextFile>
-   welcome to my game
-   tell me, what made you play this dumb game? 
*   it seemed thrilling -> Thrill 
*   this app seemed really fake -> Fake 
*   well summoning a demon seemed fun -> Thrill 

-> END

=== Thrill ===
-   ~PlayerNickname = "little bird" 
-   a thrill seeker 
-    maybe our {PlayerNickname} is a bit destructive
-   maybe our {PlayerNickname} needs someone a little more possessive 
*   maaaybe
    <sprite=12> 
    you seem like you're eager to sign your soul to me. 
    **  'm just having some fun
         well you seem like a bit of fun to play with
        ->DONE
    **  who are you?
        maybe a better question is what am I? 
        but you invited me here
        *** i didn't know that was your name
            my silly {PlayerNickname}
            were you nervous when you were summoning me? 
            ****    i'm not scared
                    your voice was so quiet. 
                    but i'm sure you'll get a little louder after we get to know each other. 
                    ->DONE
            ****    yes
                    -> YesScared     
        *** you're {DemonName} 
            ->Nervous
        *** that's right you'll serve me
            ->RoleDominated
        *** Dantadan
            it's Dantalian 
            ->Nervous
            
-> DONE

    = YesScared
    -   aww i don't want you to feel uncomfortable
    -   sometimes summons can be intense
    -   remember that circle you traced at the very start? 
    *   not really
        you'll see it everytime you launch this app. 
        it keeps you safe and will stop me from being completely with you in any other way.
        **  what if I want you out?
    *   oh i remember that
        unlucky for me it will stop me from being completely with you in any form outside of this app. 
        **  what if I want you out?
    
    -   {PlayerNickname} I am {DemonName} of hell itself. 
    -   I don't know if you've though through what that would mean.
    *   that excites me even more
        well that may be moving a bit fast
        let's start by binding your sould to me
        
    ->DONE

    = Nervous 
    -   When I heard your voice summon me, it sounded like you mumbled the pronunciation a bit. 
    -   and now you can't even spell it. 
    -   our {PlayerNickname} might need to practice typing it 
    *   Dantalian
        one more time. 
        **  Dantalian
            Good bird. 
            I have a reward for you
    
    ->DONE


=== Fake === 
-   ~PlayerNickname = "little believer" 
-   I always believed I was quite real. 
-   what's stopped you from closing the app so far? 
*   Look im just here to have some fun -> Thrill 
*   just passin the time
    I'll have to impress you to keep ya around. 
    So, {PlayerNickname}, do you summon a lot of demons in your spare time? 
    **  this is my first time -> DONE
    **  you're the third demon I summoned today
        maybe you're the true insatiable demon 
        *** how many people have summoned you today?
            Demons never text and tell ;) 
            besides the point what made you summon me 
            ****    you were my only option
                    fate has already tied us together
                    do you want to bind us more? 
                    ->DONE
        *** Let's see how long you can keep me entertained -> RoleDominated

->DONE

=== RoleDominated  ===
-   ~PlayerNickname = "believer" 
-   hmm maybe binding myself to you would be even more exciting.
->DONE