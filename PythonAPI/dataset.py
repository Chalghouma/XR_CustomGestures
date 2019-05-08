import numpy as np
from os import listdir
from os.path import isdir,isfile, join
import os 
import random
import sys 

def numpy_array_from_file(file_name):
    array = []
    bytes_read = open(file_name, "rb").read()
    count =-1
    row=[]
    for b in bytes_read:
        count = count +1
        if(((count % 56) == 0 )  and (count != 0 )):
            array.append(row)
            row = []
        row.append(b)
    array.append(row)
    return np.array(array)


def get_files(folderPath):
    onlyfiles = [f for f in listdir(folderPath) if isfile(join(folderPath, f))]
    result_list = []
    for file in onlyfiles:
        result_list .append(join(folderPath,file))
    return result_list

def get_all_categories_shuffled(root_folder):
    l_folder = join(root_folder, "L")
    circle_folder =join(root_folder,"Circle")
    arrow_folder =join(root_folder,"RightArrow")

    all_files = []
    for f in get_files(l_folder):
        all_files.append(f)
    for f in get_files(circle_folder):
        all_files.append(f)
    for f in get_files(arrow_folder):
        all_files.append(f)
    
    random.shuffle(all_files)
    all_bytes=[]
    for ff in all_files:
        all_bytes.append(numpy_array_from_file(ff))
    return (np.array(all_bytes) , [ fName.split("\\")[-2]  for fName in all_files ])

def prepare_data(root_folder, labels_dictionary , width,height ):
    images,labels = get_all_categories_shuffled(root_folder)
    length = len(images)
    for i in range(0, length ):
        labels[i] = labels_dictionary[labels[i]]
    labels=np.array(labels)
    images = np.array(images).reshape(length,width,height,1)
    images=images/255

    return (images,labels)


def load_from_dataset(dataset_name,width,height):
    images_rootfolder = join(os.path.dirname(os.path.realpath('__file__')) , 'Datasets',dataset_name,'Images')
    categories_subfolders = []
    label_mapper = []
    print('images_rootfolder : {}'.format(listdir(images_rootfolder)))

    #We get the subdirectories like : Dataset/DS/Images/Circle , Dataset/DS/Images/RightArrow , ...
    for sub_directory in listdir(images_rootfolder):
        sub_directory_fullpath = get_fullpath(join(  'Datasets',dataset_name,'Images',sub_directory))
        if (isdir(sub_directory_fullpath)):
            categories_subfolders.append(sub_directory_fullpath)
            label_mapper.append(sub_directory)


    print('Label Mapper : {}'.format(label_mapper))
    all_files = []
    #We append the paths of all the images existing in the previous subfolder
    for category_folder in categories_subfolders:
        for image_path in get_files(category_folder):
            all_files.append(image_path)

    random.shuffle(all_files)

    binarydata_of_images = []
    numerical_labels = []
    for image in all_files:
        print(image)
        binarydata_of_images.append(numpy_array_from_file(image))
        
        category = os.path.basename( os.path.dirname(image))
        index = label_mapper.index(category,0,len(label_mapper))
        numerical_labels.append(index)

    numerical_labels = np.array(numerical_labels)
    length = len(binarydata_of_images)
    print('Length of BinaryData : {}'.format(length))
    binarydata_of_images=np.array(binarydata_of_images)
    binarydata_of_images=binarydata_of_images.reshape(length,width,height,1 )
    binarydata_of_images = binarydata_of_images/255

    return (binarydata_of_images , numerical_labels , label_mapper)

def get_fullpath(path):
    return join ( os.path.dirname(os.path.realpath('__file__'))   , path)
