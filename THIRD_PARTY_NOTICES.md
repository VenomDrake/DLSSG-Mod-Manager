# Third-party components

DLSSG Mod Manager is an independent application. It is not affiliated with or endorsed by NVIDIA, Intel, AMD, Valve, Microsoft, or the authors of DLSSG for SM86, ANR or OptiScaler. It is not an official OptiScaler manager.

The manager downloads complete release assets directly from [sdli1995/dlssg_for_sm86](https://github.com/sdli1995/dlssg_for_sm86) at runtime. It does not redistribute those DLLs, models or kernels in its source, portable download, or installer. Downloaded packages retain their upstream `LICENSE.md` and `THIRD_PARTY_NOTICES.txt` in the verified cache. The manager never executes an installation script from a downloaded repository.

The manager's MIT license applies to the manager's original code only. Consult the downloaded package's notices for its NVIDIA, Intel and other third-party material. Upstream documentation and package license statements are not fully consistent in release 0.4.1; this project does not resolve or relicense that material.

ANR support downloads the selected Full or Addon package directly from [Apex Compute / ANR](https://github.com/apex-compute/ANR) at the user's request. ANR DLLs, model packs and OptiScaler binaries are not included in this manager's source, portable ZIP or MSI. Apex Compute retains credit and rights for ANR; the public repository currently does not specify a general license for ANR code, binaries or model packs. This manager does not copy, relicense or claim ownership of them. Review upstream terms before use or redistribution. The bundled upstream copy of OptiScaler is GPL-3.0; its license remains in the verified download cache and corresponding source is linked from each ANR release. The manager does not execute upstream installer scripts.

The self-contained executable contains the Microsoft .NET runtime and Windows Desktop libraries, distributed under their respective .NET Foundation/Microsoft licenses. `DOTNET-RUNTIME-LICENSE.txt`, `WPF-LICENSE.txt` and `DOTNET-THIRD-PARTY-NOTICES.txt` accompany the binaries; source copies are in `docs`. Build tooling uses the WiX Toolset 4.0.6; it is not part of the running application. Review these notice copies when updating the bundled runtime.

Standalone OptiScaler support downloads the official stable release directly from [optiscaler/OptiScaler](https://github.com/optiscaler/OptiScaler). Credit remains with cdozdil, FakeMichau, TheRazerMD and the OptiScaler contributors, and the authors of its bundled components. OptiScaler is [GPL-3.0](https://github.com/optiscaler/OptiScaler/blob/v0.9.5/LICENSE); corresponding source for the reviewed stable package is [tag v0.9.5](https://github.com/optiscaler/OptiScaler/tree/v0.9.5). Its DLLs and third-party SDK DLLs are not included in this manager's portable ZIP or MSI. The complete archive is retained as verified data, with its license notices, and native component notices are installed with the selected provider. The manager never executes its setup scripts. The stable OptiScaler package and ANR's bundled loader use different installation recipes; neither is represented as the other's official release.

## SharpCompress 0.50.9

The manager includes [SharpCompress 0.50.9](https://github.com/adamhathcock/sharpcompress/tree/0.50.9) for managed 7z decompression. It does not require a separate 7-Zip installation. SharpCompress is MIT-licensed; its [license](https://github.com/adamhathcock/sharpcompress/blob/0.50.9/LICENSE.txt) and package README also retain the following notices:

Copyright (c) 2014 Adam Hathcock. NuGet package copyright (c) 2025 Adam Hathcock.

Copyright (c) 2000 - 2011 The Legion Of The Bouncy Castle (http://www.bouncycastle.org).

The embedded Zstandard implementation is based on [ZstdSharp](https://github.com/oleg-st/ZstdSharp), as credited by SharpCompress. [ZstdSharp's MIT license](https://github.com/oleg-st/ZstdSharp/blob/master/LICENSE): Copyright (c) 2021 Oleg Stepanischev.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### Zstandard

The embedded Zstandard implementation retains the upstream [BSD license](https://github.com/facebook/zstd/blob/v1.5.7/LICENSE):

Copyright (c) Meta Platforms, Inc. and affiliates. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
* Neither the name Facebook, nor Meta, nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Product names and trademarks belong to their respective owners.
