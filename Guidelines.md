# Guidelines for thesis, internships, research work

If you have any suggestion to add to this document, create a pull request.

## Before starting

- Look on the web for information about your project. Someone may have some preliminary results that are useful to start with.
- Use Google for general research (blog posts, code, etc.)
- Use specialized search engines for scientific papers
  - Google Search
  - Scopus: <https://www.scopus.com/home.uri>
  - Web of Science: <https://www.webofscience.com/wos/woscc/basic-search>
- Most of the time you can access restricted areas/websites/documents using your Sapienza ID and password.
  - Usually, the button is named "Institutional Sign In" or "Sign in via your organization"
  - Then select IDEM (Italian online identity management for education), eIDAS or directly Sapienza university, if available
- If you are denied the download of a PDF of a research publication, ask others in the research group, someone might have the PDF.
- Join the Telegram group to coordinate with others!

## While working on your project

- Always take care about literature, participation and reproducibility!

### Literature

- If you see an interesting paper/publication/presentation or blog post, save it!
  - We use Zotero to organize papers/websites/publications/etc. https://www.zotero.org/groups/5413823/netseclab_sapienza
- Keep a note of every test and result!
- Save your code in our GitHub organization (either in your repository OR in an existing repository, if any)
  - If you never used Git, look for slides and lecture recordings about Git in the Web and Software Architecture (ACSAI, Sapienza): <http://gamificationlab.uniroma1.it/en/wasa/>
  - Commit and push often. A git commit is free of charge, and it is a good backup (plus: it's easier to share the code)
  - You should also include Jupyter notebooks, CSVs, etc.
  - If you have a big dataset, do not put it in Git, let's talk and find another place.

### Participate in the weekly meetings

- Every Thursday at 10am, here: <https://meet.google.com/cwx-bpne-ecm>
- It is ok if you don't have new results to present!
- Listen to the work of other students, and ask questions or provide suggestions if you have them.
- If you can't join the meeting, that's not a problem. However, if you miss more than one meeting, it is ‎better if you let us know ("ghosts" students may have their topic re-assigned)
- If you are stuck, **do not wait** for the meeting: send us an email and let us know. We will look for solutions together.

### Reproducibility

- Other people **MUST** be able to download your code and execute all the tests you performed.
- Write a README with instructions on how to set up a proper environment and how to launch your code to obtain the result you have.
  - Golden rule: we (as advisors) should be able to reproduce your results under these constraints:
    - MAX 30 minutes of manual preparation (install tools, libraries, etc).
    - tests/analysis/etc. MUST be automatic (bash/python scripts, jupiter notebooks, etc.). We should launch one script and that's it.
    - We will read the README for anything. If we are stuck because something is not in the README, the "reproducibility test" failed.
- All source code must be present under `netsecuritylab` organization on GitHub
- Data must be in the GitHub repository (case-by-case exceptions for big data).
- Any plot you produce should have its own pair script---data file: one file with the raw data (.csv is advised) and one for the plot (and image) generation.
- You MUST use Docker for reproducible environments.
  - Describe all containers you need using a Docker Compose.
  - For scripting, you MUST use bash/sh/zsh (or jupiter notebooks w/ python). If the script is >200 lines, write a Python or Go program instead.
  - If you make it easy to set up the environment and do the same tests, it will be easier for others to help you when you are stuck on a problem.
- Reproducibility is **required** to complete your internship/thesis.

## Writing the thesis / final report

- Start writing your thesis when you still have time!
  - Suggestion: at least 1 month before the deadline
  - We will require some days to review it, and you will require some days after that to fix it. Plan for these.
- Use LaTeX and Overleaf, if possible. It will help us speed up the process of reviewing your thesis.
  - Share the document with us. We just need the "read only link" (with the possibility of adding comments, if you see the checkbox for that)
- Technical suggestions for your thesis
  - Use PDF or EPS for images, if possible (e.g., draw.io, gnuplot, pyplots and others, they can export directly in these formats). This helps avoid scaling issues when resizing your images.
    - DO NOT convert PNGs/JPEGs to PDF/EPS. Export directly in PDF or EPS from the original tool, if possible
  - Look at good practices for LaTeX.
  - Use `\ref{}` and `\label{}` to refer to other parts of the document
  - Use `[htb]` for figures. Do not force images using `H` or `!H`. You can optionally use `p` to indicate that the figure can be in a dedicated page.
  - Do not paste tables as images, write them in LaTeX. If you need a GUI for LaTeX tables, here: <https://www.tablesgenerator.com/>
  - Do not force a new line using `\\`. Leave an empty line to create a paragraph, or just continue in the same paragraph as before.
  - Be sure that labels and text in plots are visible.
  - You may consider using `hidelinks` options in `hyperref`.
  - Use `sapthesis` template.
- English (required for master thesis)
  - You are not required to use the passive form. So, you can write your thesis as "In Chapter X, I present YZ" instead of "Chapter X presents YZ".
  - Do not overcomplicate phrases. English is a very simple language. (e.g., do not write something like "In Chapter X, YZ are presented" - see above).
  - Use Grammarly, WordTune, LanguageTools or other tools like them to check for small typos. Some of them also provide you with suggestions for phrases.
- To write a thesis:
  - First, write down a draft of a table of content. It won't be the final ToC, but it's useful when planning. Share it with the prof.
  - Then, start writing. You can start from the introduction, or from the central core of your thesis.
    - Do not write the conclusions until the end
    - Do not try to fix small graphics issues like wrong spacing between paragraphs, etc
  - At the end, fix all small graphics glitches.
- Suggested structure (but YMMV):
  - **Abstract**: brief explanation of your thesis
  - **Introduction**: introduce to the environment where the problem exists, and why solving the problem is important
  - **Related work**: previous works on this topic (mostly in scientific literature, but also on the web), including work on which you are building your solution; similar solutions, or different solutions to the same problem or similar problem, if they exists)
  - Core part of your work (including the results!)
  - **Conclusions and limitations**: recap your thesis, provide a summary of the results, indicate the limitations of your work (e.g., "this work focuses on X. In the future, Y may be investigated to extend the [...]").
  - **References**
- **There is no minimum number of pages**. However, it is unlikely that you will write less than 40 pages. If you are below 40-50 pages, do NOT try to have more pages by tweaking the spaces. The problem is the **content of your thesis**, not the design. Be sure that:
  - The introduction is at least 2 pages (people reading the thesis may be from completely different areas, like computer logic, so be sure to write a complete introduction)
  - Conclusions is at least one page, or more (recap your thesis by citing the problem, your solution and the results supporting your solutions)
  - In the related work, explain briefly the work you found
  - In the core of your thesis work, be sure to explain every decision you made, and provide some backing for that decision (empirical data, other works claiming that the direction is good, etc.)
  - Recap data, ideas and results in plots, drawing, tables; people may lack time to read your thesis fully: figures and tables help them to grasp your work
- **Do not include your source code in the thesis**, unless you want to explain some algorithm. In that case, you can include the part of the code of the algorithm instead of the pseudo code.
- Some useful links (some are in Italian, use Google Translate/Deepl)
  - <https://users.dimi.uniud.it/~stefano.mizzaro/dida/come-non-scrivere-la-tesi.html>‬
  - <https://pages.di.unipi.it/grossi/vademecum.html>‬

## Presenting your work ("defending")

- Write a good number of slides. If you spend on average 1 minute per slide, write 15 slides for 15 minutes.
- You have 15+5 minutes in your oral discussion.
- Try presenting your slides to someone: your best friend, your dog, your mirror. Use a chronometer and measure your speed.
- Your slides should contain images, tables, figures. No text, except for labels and table contents.
  - People won't be able to read the slide AND listen to you at the same time. Research shows that, if you embed text in slides, the audience won't be able to follow your presentation and they will have a bad impression of you.
- "Less is more"
- Try to answer all questions. If you don't know the answer, say "I don't know".
  - Sometimes the question is on a topic that is marginal to you. For example, if you do research on network protocol security and someone asks for UI/UX, you can easily answer that it is not in the scope of your work. E.g.
    - Q: "What about sending feedback to the user about a security problem in the network protocol?"\
       A: "Thanks for the question. It is an interesting idea, and surely we may work on that. However, it is outside the scope of this work, so I haven't tested it."
- Before answering, repeat the question aloud. It helps you to process the question.
- Export the presentation in PDF and have a "backup"
  - Send the PDF to a friend (with a PC!) or a colleague
  - Be sure that the PDF version is good for presenting
- Test your presentation on the projector in the room!
- Useful links:
  - <https://www.youtube.com/watch?v=Iwpi1Lm6dFo>
