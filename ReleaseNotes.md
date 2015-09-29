**Release Notes**
=================
v1.0
----
### Usage:
`hash-checker [/w] [/m] [/s] [/S] file1 [file2] [file3]...[/o] [output file]`

- **/w** - Specifies that the output has to be dumped to a file rather than console
- **/m** - Generates the MD5 hash
- **/s** - Generates the SHA1 hash
- **/S** - Generates the SHA256 hash
- **file** - Specifies the input files whose hashes have to be calculated (at least one file name must be specified)
- **/o** - Used to set a user defined file name for the output file. **Can be used without /w but needs the output file for expected results.**
- **output file** - Specifies a user defined output file. **Treated as an input file if /o is not used.**

#### NOTES:
- **/w** has to be the first argument
- **output file** should be specified and must be the last argument if **/o** has been used. See case 3 in **Output**.
=======
- ***file*** - Specifies the input files whose hashes have to be calculated (at least one filename must be specified)
- **/o** - Used to set a user defined filename for the output file. **Can be used without /w but needs the *output file* for expected results.**
- ***output file*** - Specifies a user defined output file. **Treated as an input file if /o is not used.**

#### NOTES:

- ****/w** has to be the first argument**
- ***output file*** **should be specified and must be the last argument if /o has been used** See case 3 in **Output**.
- The **/m**, **/s** and **/S** can be used together in any order
- In the absence of any of **/m**, **/s** or **/S**, the program defaults to generating all the three hashes.

### Output:
- If you specified the **/w** option then the requested hashes will be written to:
  1. A file with the same name and path as the last input file that you specified. `hash-checker /w D:\file1 D:\file2` will cause the output to be written to `D:\file2.checksum`

  2. The **output file** that you had specified if you had also used **/o**

  3. `o.checksum` if you did not specify **output file** when using **/o**

- When none of the arguments specifying the hash technique are passed the program defaults to generating all the three hashes
- If the input file does not exist, a message is displayed stating the same

#### Known issues:
- If the output file already exists, then the program will simply append its
    output to the end of the file.

v2.0
----
### Usage:
`hash-checker [/w] [/m] [/s] [/S] file1 [file2] [file3]...`

`hash-checker [/write-to-file] [/md5] [/sha1] [/sha256] file1 [file2] [file3]...`

`hash-checker [-w] [-m] [-s] [-S] file1 [file2] [file3]...`

`hash-checker [--write-to-file] [--md5] [--sha1] [--sha256] file1 [file2] [file3]...`

- **w** or **write-to-file** - Specifies that the output has to be dumped to a file rather than console
- **m** or **md5** - Generates the MD5 hash
- **s** or **sha1**- Generates the SHA1 hash
- **S** or **sha256**- Generates the SHA256 hash
- **file** - Specifies the input files whose hashes have to be calculated (at least one file must be specified)

#### NOTES:
- The arguments can be used together in any order (but must be specified before the input files)
- In the absence of any of **md5**, **sha1** or **sha256**, the program defaults to generating all the three hashes

### Output:
- If you specified the **/w** option then the requested hashes will be written to a file with the same name and path as the last input file that you specified. `hash-checker /w D:\file1 D:\file2` will cause the output to be written to `D:\file2.checksum`
- When none of the arguments specifying the hash technique are passed the program defaults to generating all the three hashes
- If the input file does not exist, a message is displayed stating the same
- Upon any kind of unexpected input a message showing the expected input is displayed and the program terminates

#### Known issues:
- If the output file already exists, then the program will simply append its output to the end of the file.

##### If you find any other issues, please file an issue at [GitHub](<https://github.com/hashhar/hash-checker/issues/new>) or e-mail me at <hashhar_dev@outlook.com>.
