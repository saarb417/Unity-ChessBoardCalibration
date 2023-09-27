import cv2
import numpy as np

import os,argparse,string,random

import xml_output,transfer_ssh



# Define the dimensions of checkerboard
CHECKERBOARD = (4, 8)

calib_path = 'C:\\Users\\saarb\\UnityProjects\\ChessboardCalibration\\Recordings\\'
# calib_path = 'C:\\Users\\saarb\\OneDrive\\Desktop\\saved\\'
save_calib = False

def get_distortion_coefficients(samples):

    # stop the iteration when specified
    # accuracy, epsilon, is reached or
    # specified number of iterations are completed.
    criteria = (cv2.TERM_CRITERIA_EPS +
                cv2.TERM_CRITERIA_MAX_ITER, 30, 0.001)

    # Vector for 3D points
    threedpoints = []

    # Vector for 2D points
    twodpoints = []

    # 3D points real world coordinates
    objectp3d = np.zeros((1, CHECKERBOARD[0]
                        * CHECKERBOARD[1],
                        3), np.float32)
    objectp3d[0, :, :2] = np.mgrid[0:CHECKERBOARD[0],
                                0:CHECKERBOARD[1]].T.reshape(-1, 2)
    true_counter = 0

    # Extracting path of individual image stored
    # in a given directory. Since no path is
    # specified, it will take current directory
    # jpg files alone
    all_images = os.listdir(calib_path)

    for i in range(samples):
        image = cv2.imread(calib_path + all_images[i])
        grayColor = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

        # Find the chess board corners
        # If desired number of corners are
        # found in the image then ret = true
        ret, corners = cv2.findChessboardCorners(
            grayColor, CHECKERBOARD,
            cv2.CALIB_CB_ADAPTIVE_THRESH
            + cv2.CALIB_CB_FAST_CHECK +
            cv2.CALIB_CB_NORMALIZE_IMAGE)

        # If desired number of corners can be detected then,
        # refine the pixel coordinates and display
        # them on the images of checker board

        if ret == True:
            true_counter += 1

            if save_calib:
                # Define the characters from which the random string will be generated
                characters = string.ascii_letters + string.digits
                name = ''.join(random.choice(characters) for _ in range(16)) + ".jpg"
                cv2.imwrite('C:\\Users\\saarb\\OneDrive\\Desktop\\saved\\'+ name, image)

            threedpoints.append(objectp3d)

            # Refining pixel coordinates
            # for given 2d points.
            corners2 = cv2.cornerSubPix(
                grayColor, corners, (11, 11), (-1, -1), criteria)

            twodpoints.append(corners2)

            # Draw and display the corners
            image = cv2.drawChessboardCorners(image,
                                            CHECKERBOARD,
                                            corners2, ret)
        else:
            print(str(i+1))

    h, w = image.shape[:2]

    # Perform camera calibration by
    # passing the value of above found out 3D points (threedpoints)
    # and its corresponding pixel coordinates of the
    # detected corners (twodpoints)
    ret, matrix, distortion, r_vecs, t_vecs = cv2.calibrateCamera(
        threedpoints, twodpoints, grayColor.shape[::-1], None, None)

    # Displaying required output

    print("relevant images: " + str(true_counter))

    print(" RMS: ")
    print(ret)

    print(" Camera matrix:")
    print(matrix)

    print("\n Distortion coefficient:")
    print(distortion)

    return distortion,matrix,ret,(h,w)

    

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Camera calibration and XML transfer.')
    parser.add_argument('--transfer_xml', action='store_true', help='Transfer XML file over SSH')
    parser.add_argument('--samples', type=int, default=50, help='Number of samples for calibration (default: 50)')
    args = parser.parse_args()

    transfer_xml_flag = args.transfer_xml
    samples = args.samples
    fps = 30

    distortion, matrix, rms, res = get_distortion_coefficients(samples)

    if transfer_xml_flag:
        xml_output.xml_creator(distortion, matrix, rms, res, fps)
        transfer_ssh.transfer_xml()


