public static class AnimationManager
{
    public static string GetAnimName(IAnimType anim)
    {
        return anim switch
        {
            IAnimType.idle => "idle",
            IAnimType.walk => "walk",
            IAnimType.run => "run",
            _ => "idle"
        };        
    }
}
