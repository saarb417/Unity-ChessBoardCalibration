import paramiko
import argparse

# recording_dir = 'C:\\Users\\saarb\\UnityProjects\\CamerasCalibration\\Recordings\\'
# remote_dir = '/home/ception/GIT/ception/UnityVSlam/data/UP_FOV=70_new/'


# SSH connection information
host = '192.168.167.90'
port = 22
username = 'ception'
password = '1234'


def transfer_xml(remote_dir):
    # Local path to the XML file you want to transfer
    local_file_path = 'unity_cam.xml'

    # Remote destination path on the SSH server
    remote_file_path = remote_dir + local_file_path

    try:
        # Create an SSH client
        ssh_client = paramiko.SSHClient()
        ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

        # Connect to the SSH server
        ssh_client.connect(hostname=host, port=port, username=username, password=password)

        # Create an SFTP client from the SSH client
        sftp = ssh_client.open_sftp()

        # Transfer the XML file
        sftp.put(local_file_path, remote_file_path)

        print(f'File "{local_file_path}" transferred to "{remote_file_path}" successfully.')

    except Exception as e:
        print(f'An error occurred: {str(e)}')

    finally:
        # Close the SSH and SFTP connections
        if ssh_client:
            ssh_client.close()
        if sftp:
            sftp.close()



def transfer_video_to_remote(videos,recording_dir,remote_dir):
    try:
        # Create an SSH client
        ssh_client = paramiko.SSHClient()
        ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

        # Connect to the SSH server
        ssh_client.connect(hostname=host, port=port, username=username, password=password)

        # Create an SFTP client from the SSH client
        sftp = ssh_client.open_sftp()

        for video in videos:

            # Construct the remote file path
            remote_file_path = remote_dir + "random_videos/" + video

            # Transfer the video file
            sftp.put(recording_dir + video, remote_file_path)

            print(f'File "{video}" transferred to "{remote_file_path}" successfully.')


    except Exception as e:
        print(f'An error occurred: {str(e)}')

    finally:
        # Close the SSH and SFTP connections
        if ssh_client:
            ssh_client.close()
        if sftp:
            sftp.close()



if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Transfer files over SSH.')
    parser.add_argument('--transfer_videos', action='store_true', help='Transfer video files')
    parser.add_argument('--videos', nargs='+', help='List of video filenames to transfer')
    parser.add_argument('--local_dir', type=str, default='.', help='Local directory containing the XML file (default: current directory)')
    parser.add_argument('--remote_dir', type=str, default='.', help='Remote directory for the XML file (default: /home/ception/GIT/ception/UnityVSlam/data/UP_FOV=70_new/)')
    args = parser.parse_args()


    if args.transfer_videos:
        if args.videos:
            videos_to_transfer = args.videos
            recording_dir=args.local_dir
            remote_dir=args.remote_dir
            transfer_video_to_remote(videos_to_transfer,recording_dir,remote_dir)
        else:
            print("No video filenames provided. Please specify the --videos option.")