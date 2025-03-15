# PDF Converter into Markdown files

This is a small .Net project that converts PDF files into Markdown files. It uses the Mistral OCR and Spectre.Console library.

## Things to do:
- [ ] Create initial menu
    - [ ] Configuration  for the Mistral API
    - [ ] Where to store it? Maybe it should be in separate file as a hashed information? For start it should be fine.
    - [ ] Options like
        - choose file
        - choose folder
        - something similar? Best would be to use something that is easy to use but also enjoyable. Maybe files explorer inside the app
- [ ] Create a class that will handle the Mistral API
    - [ ] method that will take in a fila and call OCR Api
    - [ ] method that will save those files on user drive in selected folder