"""Render the vector brand into PNG and Windows ICO (developer-only ImageMagick)."""
from pathlib import Path
import shutil
import struct
import subprocess

root = Path(__file__).resolve().parents[1]
assets = root / "src/DlssgManager.Desktop/Assets"
magick = shutil.which("magick")
if not magick:
    raise SystemExit("Install ImageMagick to regenerate the icon. Normal app builds use the checked-in assets.")
subprocess.run([magick, "-background", "none", "-density", "192", str(assets / "icon.svg"),
                "-resize", "512x512", "-strip", "PNG32:" + str(assets / "app.png")], check=True)
sizes = (16, 20, 24, 32, 40, 48, 64, 96, 128, 256)
frames = []
for size in sizes:
    source = assets / ("icon-small.svg" if size <= 24 else "icon.svg")
    frames.append(subprocess.run([magick, "-background", "none", "-density", str(size * 96 / 256),
                                  str(source), "-resize", f"{size}x{size}", "-strip", "PNG32:-"],
                                 check=True, capture_output=True).stdout)
# Windows 10/11 supports PNG-compressed ICO entries. Only package the rendered
# images here; the SVG sources own the artwork, including the compact-size hinting.
offset = 6 + 16 * len(sizes)
directory = bytearray(struct.pack("<HHH", 0, 1, len(sizes)))
for size, frame in zip(sizes, frames):
    directory += struct.pack("<BBBBHHII", size % 256, size % 256, 0, 0, 1, 32, len(frame), offset)
    offset += len(frame)
(assets / "app.ico").write_bytes(directory + b"".join(frames))
print("Generated app.png and app.ico from the SVG sources (16-256 px Windows icon frames).")
