from tkinter import *

root = Tk()
root.title("Hello Tk")

root.geometry("800x600")

label = Label(root, 
    text="Hello Label", 
    width=100, 
    height=50, 
    relief="solid", 
    bitmap="info", 
    compound="top")

label.pack()

root.mainloop()