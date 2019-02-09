
using System.Collections.Generic;

namespace WyeCore {

  /*
  What are our types? Our fundamentals? Our "primatives"?
  That's a hard question to answer. We have to have a set of primitives that
  are used to construct all of the other types in the language, but what should
  they be?

  Obviously we need some kind of list, and we need some kind of dictionary. We
  also need some simpler types, like strings and numbers, right? Perhaps we
  could construct them all out of bytes? No, we can't construct them out of
  doing so would apply "artifical" restrictions to their size/function that
  are really only necessary to achieve high performance... they're not
  something one should be thinking about while proving out the functional
  elements of their program.

  SO, we need a number primitive? And a string primitive? Maybe just a char
  and lists? The only reason not to do a list of chars is to make converting
  certain syntax into such more... "core" to the language, but I don't see any
  reason that the backend implementation needs to be "built in".

  So, primitives:
    - integer
    - character
    - sequence
    - dictionary
  
  Is that it? It seems like we should be able to make a capable compiler with
  just those. I MIGHT throw bytes on there, but I think I'd rather not have the
  core language be concerned with bytes. That's implementation specific and
  language specific and platform specific.

  All of these types would be called nuggets. Rather a glob of data is called a
  nugget (after all, data is the new "gold" ;) ).

  AH YES! Should a dictionary be a primitive, or should it be implemented with
  a list? Technically a map can be constructed from the other elements, it's
  just a question of how much work is it to do the rest? Yes, a dictionary
  should be a primitive, or at least a built in "interface". I think a
  dictionary is used to implement so many other things that it would create a
  large amount of work to get to a stable working language without a dictionary
  provided.

  
   */
  public interface Nugget {
    
  }

  public class Integer : Nugget {

  }

  public class TextCharacter : Nugget {

  }

  public class Sequence<T> : List<T>, Nugget {

    public Sequence(IEnumerable<T> source): base(source) {}

  }

  public class Dictionary : Nugget {

  }
}