#include <semaphore.h>

// Delegate functions for semaphore (MacOS) since it doesn't work with P/Invoke

sem_t *gx_sem_open(const char *name, int oflag, mode_t mode, unsigned int initial_count)
{
    return gx_sem_open(name, oflag, mode, initial_count);
}

sem_t *gx_sem_open_or_create(const char *name, unsigned int initial_count)
{
    return sem_open(name,
        O_CREAT, // create the semaphore if it does not exist
        S_IRWXU | S_IRWXG | S_IRWXO, // Maximum permission (RWX for all)
        initial_count);
}

sem_t *gx_sem_open_existing(const char *name)
{
    return sem_open(name, 0); // 0 means open but not create it
}

int gx_sem_post(sem_t *sem)
{
    return sem_post(sem);
}

int gx_sem_wait(sem_t *sem)
{
    return sem_wait(sem);
}

int gx_sem_trywait(sem_t *sem)
{
    return sem_trywait(sem);
}

int gx_sem_close(sem_t *sem)
{
    return sem_close(sem);
}

int gx_sem_unlink(const char *name)
{
    return sem_unlink(name);
}
