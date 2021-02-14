
<p align="center">
  <img width="260" height="310" src="./wye.svg">
</p>

## wye - C#

This is an interpretter for the wye language, written in C#.

# wye

Why is there yet another programming language? For every task that is out there, there is a language that handles it well. For every language out there, there are many tasks that it handles poorly. For every task out there, wye aims to handle it well. Wye is the penultimate letter and aims to be the penultimate language. Too justify a new language, it must be foolishly ambitious.

Why should you use wye? Why not? While it would be wonderful if wye could outperform all other languages in all tasks, such a goal is never to be reached. Wye's main focus is to be the ultimate language for writing libraries. It focuses on code clarity, flexibility, performance, and cross-language compatibility. As a stretch goal, it aims to be highly capable as a configuration language, a service language, a DB schema definition, a structured data protocol for network communications, a top-level micro-service language, a server back-end language, a scripting language, and even a front-end application language. To sum it all up, it aims to produce more reliable code more quickly than any other language with the highest degree of clarity in its interfaces. If it achieves this objective, it should gradually replace the entire computer programming stack, starting at the lowest levels of operating systems and system drivers, typically written in C with the highest degree of reuse and highest need for reliability, gradually climbing up the stack to applications, and eventually to the most modern and stylish graphical interfaces.

## Now

Right now this project is a potato.

## Eventually

### Flexible Constraints
With extreme "typing" flexibility, it handles quick and dirty scripting yet scales up to a dev scale of thousands (thousands of collaborators) better than any classic type system. It allows for lax interfaces when a design is still being nailed down or the dev scale is 1, yet then allows more clear interfaces than any type system currently implemented as the need arises. Wye is a scripting language and a systems language.

### Explicit Externalities
Every external dependency and side-effect of a function is called out explicitly. By default functions are functional, with no externalities other than the inputs and the output. It's easy to substitute alternative or mock services for testing. It's easy to know what the affect of a function is. This allows for wye code to be executed during compilation of wye code. This allows for wye to be used in configuration files, as most functions used (such as math functions) can be guaranteed to be functional. Even for functions with dependencies, the dependencies could be provided to the configuration file to evaluate. Wye is not only a programming language.

### Alternative implementations
In many circumstances in programming, there is a tradeoff between increasing performance and maintaining code flexibility and readability. Wye supports multiple implementations of functions and services, allowing for a reliable and readable first implementation and an optimized second or third implementation. All of the implementations can coexist. Not only does this allow for performant code to have a more readable expression of the raw meaning, it allows for the optimization to be compared to the less confusing version to guarantee neither were any mistakes made during optimization nor does the optimization fail to provide substantial performance benefits. Wye is simultaneously performant, readable, and reliable.

### Language Translation
Wye compiles into other languages. Automatic language translation focuses on preserving functionality, not performance. However, alternate implementations of functions and services could be implemented in a target language, allowing for improved performance or support for more coupled integration as necessary. Wye can be implemented into any codebase at any time.

# Install

## Windows

1. Clone wye using the command `git clone https://github.com/semimono/wye-csharp.git`
2. Open the project in Visual Studio 2017
3. Build the project with the configuration set to "Release"
4. Copy the contents of the `Wye\bin\Release` folder (relative to the project root directory) into a program folder somewhere, for example `C:\wye`
5. Change your system path variable to include the wye program folder, i.e. `C:\wye`

## Linux

1. Clone wye using the command `git clone https://github.com/semimono/wye-csharp.git`
2. Open a terminal and run `sudo apt-get install nuget`
3. Run `sudo ./install.sh` and restart the terminal

# Use
