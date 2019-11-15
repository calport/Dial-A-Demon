//Day 3 Never Have I ever
VAR NumberDemon = 0 
VAR NumberPlayer = 0

-   Wanna play a game? 
*   isn't that how this all started?
-   is that a yes? 
*   what game do you want to play?
-   never have I ever? 
-   we'll play a short 3 strike version
*   i think you might be combining games
-   whatever, you get the gist. 
-   I'm guessing you know how to play
*   yep! im ready to take you down!
-   We'll see about that 
-   Never have I ever sent anyone a sexy pic
*   you're just jumping straight in
-   is that a strike? 
*   nope! Still safe! 
    There's still time! 
    **   my turn!!!
*   yep! 
    ~ NumberPlayer = NumberPlayer +1 
    maybe you can send me one, sometime 
    **  pick truth or dare next time
    **  it was just once
    --   okay okay

-   you're turn ! 
*   I have never roleplayed
    ~NumberDemon = NumberDemon+0
    depends what type your talking about... 
    but I've always stayed myself
    turns out demons are pretty multifaceted 
    **  dang it!
*   Never have I ever vacationed in purgatory
    okay, that really feels like a lucky guess
    like sometimes it seems the only thing you humans got right is the name to things... 
    fine. 
    **  so is that a strike?
    **  lucky guesses still count!

-   ~NumberDemon = NumberDemon+1
-   never have I ever fallen asleep in the movies. 
*   that's a little less interesting
    I'm not playing to lose
    **  fine I have
    **  well you'll have to try harder. 
*   who hasn't? 
*   I would never. 

-
*   Never have I ever been 6ft under
*   I've never been caught looking at anything naughy 
        Discretion is part of my what I do. 
        Lust demons can't just go around sharing what's bound to them.
*   Have you ever stolen anything? Because I haven't 
    ~NumberDemon = NumberDemon +1 
    Okay fine. 
    my turn. 
    **  Not going to share? 
        Nope! gotta keep a mysterious profile
    ** Ready!

-   hmmm 
-   {   NumberPlayer > 1: 
            Never have I ever worn clothing to cover a hickey
        -   else:
            Never have I ever signed a contract before. 
            *   that's not fair 
                fairness isn't going to help me win.
                ->PlayerThirdQuestion
        
    }
    
-   (PlayerThirdQuestion)but go ahead keep playing fair for me. 
-   {NumberDemon >1: 
        *   Never have I ever.... 
            this is option
            ->DemonFourthQuestion
        *   Option 2 
                kadjflajd
            ->DemonFourthQuestion
    -   else:
        *Never have I ever 
            nextion option
            -> DemonFourthQuestion
}

- (DemonFourthQuestion)
->DONE

