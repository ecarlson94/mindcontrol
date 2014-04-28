--------------------------------------------------------------------------------
About this Content Project
--------------------------------------------------------------------------------

The content project "Content" contains content for the XNA Win and Xbox samples.
"Content(Reach)" contains content for the samples which only use the "Reach"
profile (e.g. the XNA Windows Phone samples).
The content project "Content(MG)" contains content for the MonoGame samples.

References
----------
Please note that these content projects reference several DigitalRune content 
pipeline assemblies: e.g. DigitalRune.Mathematics.Content.Pipeline.dll, 
DigitalRune.Geometry.Content.Pipeline.dll, etc.

Content Processors
------------------
Most of the models use the DigitalRune Model Processor and not the default XNA 
model processor! Please check the Content Processor settings (in the Visual 
Studio Properties window) to see how an individual asset is processed.

MonoGame Content
----------------
The content projects "Content(MG)" is for use with MonoGame projects. 
"ContentBuilder(MG)" is an XNA project which builds the "Content(MG)" 
content project (and produces a dummy DLL).
The main difference between normal XNA content projects and the MonoGame content
projects is that the MonoGame content project reference the 
MonoGameContentProcessors DLL. 
If you add an effect (.fx) to the content projects, make sure that the 
effect in the MonoGame content projects uses the MGEffectProcessor (from 
MonoGameContentProcessors.dll) instead of the normal XNA EffectProcessor.
